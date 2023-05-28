using ChartTools.Animations;
using ChartTools.Extensions;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

using System.Runtime.CompilerServices;

namespace ChartTools.IO.Midi.Parsing;

internal abstract class LaneInstrumentParser<TChord, TLane, TModifier> : LaneInstrumentParser<TChord, LaneNote<TLane>, TLane, TModifier>
    where TChord : LaneChord<LaneNote<TLane>, TLane, TModifier>, new()
    where TLane : struct, Enum
    where TModifier : struct, Enum
{
    protected LaneInstrumentParser(ILaneInstrumentReadMapper mapper, ReadingSession session) : base(mapper, session) { }
}

internal abstract class LaneInstrumentParser<TChord, TNote, TLane, TModifier> : InstrumentParser<TChord>
    where TChord : LaneChord<TNote, TLane, TModifier>, new()
    where TNote : LaneNote<TLane>, new()
    where TLane : struct, Enum
    where TModifier : struct, Enum
{
    protected readonly Dictionary<Difficulty, Dictionary<TLane, TChord?>>
        openedNoteSources = new(from difficulty in EnumCache<Difficulty>.Values
                                let pairs = from lane in EnumCache<TLane>.Values
                                            select new KeyValuePair<TLane, TChord?>(lane, null)
                                select new KeyValuePair<Difficulty, Dictionary<TLane, TChord?>>(difficulty, new(pairs)));

    protected readonly Dictionary<Difficulty, TChord?> previousChords = new(from difficulty in EnumCache<Difficulty>.Values
                                                                            select new KeyValuePair<Difficulty, TChord?>(difficulty, null));

    protected readonly Dictionary<Difficulty, Dictionary<TrackSpecialPhraseType, uint?>>
        openedSpecialPositions = new(from difficulty in EnumCache<Difficulty>.Values
                                     let pairs = from type in EnumCache<TrackSpecialPhraseType>.Values
                                                 select new KeyValuePair<TrackSpecialPhraseType, uint?>(type, null)
                                     select new KeyValuePair<Difficulty, Dictionary<TrackSpecialPhraseType, uint?>>(difficulty, new(pairs)));

    protected readonly Dictionary<TrackSpecialPhraseType, uint?> openedSharedTrackSpecialPositions = new(from type in EnumCache<TrackSpecialPhraseType>.Values
                                                                                                         select new KeyValuePair<TrackSpecialPhraseType, uint?>(type, null));

    private readonly Dictionary<int, uint?>? openedBigRockPositions;
    private readonly List<InstrumentSpecialPhrase> bigRockEndings = new();

    protected virtual byte BigRockCount => 0;

    public virtual ILaneInstrumentReadMapper Mapper { get; }

    public LaneInstrumentParser(ILaneInstrumentReadMapper mapper, ReadingSession session) : base(session)
    {
        Mapper = mapper;

        if (BigRockCount > 1)
            openedBigRockPositions = new(from index in Enumerable.Range(1, BigRockCount)
                                         select new KeyValuePair<int, uint?>(index, null));
    }

    protected override void HandleItem(MidiEvent item)
    {
        globalPosition += (uint)item.DeltaTime;

        if (item is TextEvent txt)
        {
            if (!CustomTextHandle(txt))
                foreach (var t in tracks)
                    t.LocalEvents.Add(new(globalPosition, txt.Text));
            return;
        }
        if (item is not NoteEvent note)
        {
            session.InvalidMidiEventProcedure(globalPosition, item);
            return;
        }

        if (!CustomHandle(note))
            foreach (var mapping in Mapper.Map(globalPosition, note))
                BaseHandle(mapping);
    }
    protected void BaseHandle(NoteEventMapping mapping)
    {
        switch (mapping.Type)
        {
            case MappingType.Special:
                HandleSpecial(mapping);
                break;
            case MappingType.Modifier:
                HandleModifier(mapping);
                break;
            case MappingType.Note:
                HandleNote(mapping);
                break;
            case MappingType.BigRock:
                HandleBigRock(mapping);
                break;
        }
    }

    protected virtual void HandleSpecial(NoteEventMapping mapping)
    {
        var track = GetOrCreateTrack(mapping.Difficulty);
        var type = (TrackSpecialPhraseType)mapping.Index;
        var openedPosition = track is null ? openedSharedTrackSpecialPositions[type] : openedSpecialPositions[track.Difficulty][type];

        switch (mapping.State)
        {
            case NoteState.Open:
                if (track is null)
                {
                    if (openedPosition is not null)
                        session.UnclosedProcedure(openedPosition.Value, () =>
                        {
                            InitTracks();

                            foreach (var track in tracks)
                                CloseSpecial(track);
                        });

                    openedSharedTrackSpecialPositions[type] = mapping.Position;
                }
                else
                {
                    if (openedPosition is not null)
                        session.UnclosedProcedure(openedPosition.Value, () => CloseSpecial(track));

                    openedSpecialPositions[track.Difficulty][type] = mapping.Position;
                }
                break;
            case NoteState.Close:
                if (track is null)
                {
                    if (openedPosition is null)
                        session.UnopenedProcedure(mapping.Position, () =>
                        {
                            InitTracks();

                            foreach (var track in tracks)
                                track.SpecialPhrases.Add(new(mapping.Position, type));

                            for (int i = 0; i < tracks.Length; i++)
                                (tracks[i] ??= new() { Difficulty = (Difficulty)i }).SpecialPhrases.Add(new(mapping.Position, type));
                        });
                    else
                    {
                        InitTracks();

                        foreach (var t in tracks)
                            CloseSpecial(t);

                        openedSharedTrackSpecialPositions[type] = null;
                    }
                }
                else
                {
                    if (openedPosition is null)
                        session.UnopenedProcedure(mapping.Position, () => track.SpecialPhrases.Add(new(mapping.Position, type)));
                    else
                    {
                        CloseSpecial(track);
                        openedSpecialPositions[track.Difficulty][type] = null;
                    }
                }
                break;
        }

        void CloseSpecial(Track<TChord> track) => track.SpecialPhrases.Add(new(openedPosition.Value, type, GetSustain(openedPosition!.Value, mapping.Position)));
        void InitTracks()
        {
            for (int i = 0; i < tracks.Length; i++)
                tracks[i] ??= new() { Difficulty = (Difficulty)i };
        }
    }
    protected virtual void HandleModifier(NoteEventMapping mapping)
    {
        var track = GetOrCreateTrack(mapping.Difficulty);
        var modifierIndex = mapping.Index;

        if (track is null)
            foreach (var t in tracks)
                ApplyModifier(t);
        else
            ApplyModifier(track);

        void ApplyModifier(Track<TChord> track) => AddModifier(GetOrCreateChord(mapping.Position, track), modifierIndex);
    }
    protected virtual void HandleNote(NoteEventMapping mapping)
    {
        var track = GetOrCreateTrack(mapping.Difficulty);

        if (track is null)
            return;

        var index = mapping.Index;
        var lane = Unsafe.As<byte, TLane>(ref index);
        var openedSource = openedNoteSources[track.Difficulty][lane];

        switch (mapping.State)
        {
            case NoteState.Open:
                if (openedSource is not null)
                    session.UnclosedProcedure(openedSource.Position, () => openedSource.Notes[lane]!.Sustain = GetSustain(openedSource.Position, mapping.Position));

                var chord = openedNoteSources[track.Difficulty][lane] = GetOrCreateChord(mapping.Position, track);

                session.DuplicateTrackObjectProcedure(chord.Position, "note", () => chord.Notes.Contains(lane));

                chord.Notes.Add(new TNote() { Lane = lane });
                break;
            case NoteState.Close:
                if (openedSource is null)
                    session.UnopenedProcedure(mapping.Position, () => GetOrCreateChord(mapping.Position, track).Notes.Add(lane));
                else
                {
                    openedSource.Notes[lane]!.Sustain = GetSustain(openedSource.Position, mapping.Position);
                    openedNoteSources[track.Difficulty][lane] = null;
                }
                break;
        }
    }
    protected virtual void HandleBigRock(NoteEventMapping mapping)
    {
        if (BigRockCount == 0)
            return;

        var openedBigRockPosition = openedBigRockPositions[mapping.Index];

        switch (mapping.State)
        {
            case NoteState.Open:
                if (openedBigRockPosition is not null)
                    throw new Exception($"Big rock ending {mapping.Index} is already present for {GetInstrument().InstrumentIdentity}."); // TODO Custom exception

                openedBigRockPositions[mapping.Index] = mapping.Position;
                break;
            case NoteState.Close:
                if (openedBigRockPosition is null)
                    session.UnopenedProcedure(mapping.Position, () => GetInstrument().SpecialPhrases.Add(new(mapping.Position, InstrumentSpecialPhraseType.BigRockEnding)));
                else
                {
                    bigRockEndings.Add(new(mapping.Position, InstrumentSpecialPhraseType.BigRockEnding, GetSustain(openedBigRockPosition!.Value, mapping.Position)));
                    openedBigRockPositions[mapping.Index] = null;
                }
                break;
        }
    }
    private TChord GetOrCreateChord(uint newChordPosition, Track<TChord> track)
    {
        var chord = previousChords[track.Difficulty];

        if (chord is null)
            return Create();
        else if (chord.Position + 10 >= newChordPosition)
            return session.SnappedNotesProcedure(chord.Position, newChordPosition) ? chord : Create();
        else
            return Create();

        TChord Create()
        {
            var newChord = new TChord() { Position = newChordPosition };
            track.Chords.Add(previousChords[track.Difficulty] = newChord);
            return newChord;
        }
    }

    protected virtual bool CustomHandle(NoteEvent note) => false;
    protected virtual bool CustomTextHandle(TextEvent text) => false;

    protected override void FinaliseParse()
    {
        if (bigRockEndings.Count > 0)
        {
            if (bigRockEndings.Count < BigRockCount && !session.MissingBigRockProcedure())
                return;

            var ending = bigRockEndings.UniqueBy(e => e.Position) || !bigRockEndings.UniqueBy(e => e.Length)
                ? bigRockEndings.First()
                : session.MisalignedBigRockProcedure(bigRockEndings);

            if (ending is not null)
                GetInstrument().SpecialPhrases.Add(ending);
        }

        base.FinaliseParse();
    }

    protected abstract TChord CreateChord(uint position);
    protected Track<TChord>? GetOrCreateTrack(Difficulty? difficulty) => difficulty is null ? null : (tracks[(int)difficulty] ??= new() { Difficulty = difficulty.Value });
    protected abstract void AddModifier(TChord chord, byte modifierIndex);

    public override void ApplyToSong(Song song)
    {
        if (Mapper is not IAnimationContainer)
            return;

        if (Mapper is IAnimationContainer<HandPositionEvent> handContainer)
            song.Animations.Guitar.AddRange(handContainer.AnimationEvents);

        if (Mapper is IAnimationContainer<VocalistMouthEvent> vocalistContainer)
            song.Animations.Vocals.AddRange(vocalistContainer.AnimationEvents);
    }
}
