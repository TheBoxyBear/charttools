using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Parsing
{
    internal abstract class LaneInstrumentParser<TChord, TNote, TLane, TModifier> : InstrumentParser<TChord>
        where TChord : LaneChord<TNote, TLane, TModifier>
        where TNote : Note<TLane>, new()
        where TLane : struct, Enum
        where TModifier : struct, Enum
    {
        protected readonly Dictionary<Difficulty, Dictionary<TLane, TChord?>>
            openedNoteSources = new(from difficulty in EnumCache<Difficulty>.Values
                                    let pairs = from lane in EnumCache<TLane>.Values
                                                select new KeyValuePair<TLane, TChord?>(lane, null)
                                    select new KeyValuePair<Difficulty, Dictionary<TLane, TChord?>>(difficulty, new(pairs)));

        protected readonly Dictionary<Difficulty, TChord?> previousChords = new(from difficulty in EnumCache<Difficulty>.Values select new KeyValuePair<Difficulty, TChord?>(difficulty, null));

        protected readonly Dictionary<Difficulty, Dictionary<SpecialPhraseType, uint?>>
            openedSpecialPositions = new(from difficulty in EnumCache<Difficulty>.Values
                                         let pairs = from type in EnumCache<SpecialPhraseType>.Values
                                                     select new KeyValuePair<SpecialPhraseType, uint?>(type, null)
                                         select new KeyValuePair<Difficulty, Dictionary<SpecialPhraseType, uint?>>(difficulty, new(pairs)));

        protected readonly Dictionary<SpecialPhraseType, uint?> openedSharedSpecialPositions = new(from type in EnumCache<SpecialPhraseType>.Values select new KeyValuePair<SpecialPhraseType, uint?>(type, null));

        public LaneInstrumentParser(InstrumentIdentity instrument, ReadingSession session) : base(instrument, session) { }

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
                session.HandleInvalidMidiEventType(globalPosition, item);
                return;
            }

            if (CustomHandle(note))
                return;

            var mapping = MapNoteEvent(note);
            var track = mapping.Track;

            switch (mapping.Type)
            {
                case MappingType.Special:
                    var type = (SpecialPhraseType)mapping.Index;
                    var openedPosition = track is null ? openedSharedSpecialPositions[type] : openedSpecialPositions[track.Difficulty][type];

                    switch (note)
                    {
                        case NoteOnEvent:
                            if (track is null)
                            {
                                if (openedPosition is not null)
                                    session.HandleUnclosed(openedPosition.Value, () =>
                                    {
                                        foreach (var track in tracks)
                                            CloseSpecial(track);
                                    });

                                openedSharedSpecialPositions[type] = globalPosition;
                            }
                            else
                            {
                                if (openedPosition is not null)
                                    session.HandleUnclosed(openedPosition.Value, () => CloseSpecial(track));

                                openedSpecialPositions[track.Difficulty][type] = globalPosition;
                            }
                            break;
                        case NoteOffEvent:
                            if (track is null)
                            {
                                if (openedPosition is null)
                                    session.HandleUnopened(globalPosition, () =>
                                    {
                                        foreach (var track in tracks)
                                            track.SpecialPhrases.Add(new(globalPosition, type));
                                    });
                                else
                                {
                                    foreach (var t in tracks)
                                        CloseSpecial(t);

                                    openedSharedSpecialPositions[type] = globalPosition;
                                }
                            }
                            else
                            {
                                if (openedPosition is null)
                                    session.HandleUnopened(globalPosition, () => track.SpecialPhrases.Add(new(globalPosition, type)));
                                else
                                {
                                    CloseSpecial(track);
                                    openedSpecialPositions[track.Difficulty][type] = null;
                                }
                            }
                            break;
                    }

                    void CloseSpecial(Track<TChord> track) => track.SpecialPhrases.Add(new(globalPosition, type, globalPosition - openedPosition!.Value));
                    break;
                case MappingType.Modifier:
                    var modifierIndex = mapping.Index;

                    if (track is null)
                        foreach (var t in tracks)
                            ApplyModifier(t);
                    else
                        ApplyModifier(track);

                    void ApplyModifier(Track<TChord> track)
                    {
                        var chord = GetOrCreateChord(globalPosition, track);
                        AddModifier(chord, modifierIndex);
                    }
                    break;
                case MappingType.Note:
                    if (track is null)
                        return;

                    var lane = ToLane(mapping.Index);
                    var openedSource = openedNoteSources[track.Difficulty][lane];

                    switch (note)
                    {
                        case NoteOnEvent:
                            if (openedSource is not null)
                                session.HandleUnclosed(openedSource.Position, () => openedSource.Notes[lane]!.Length = globalPosition - openedSource.Position);

                            var chord = openedNoteSources[track.Difficulty][lane] = GetOrCreateChord(globalPosition, track);

                            session.DuplicateTrackObjectProcedure(chord.Position, "note", () => chord.Notes.Contains(lane));

                            chord.Notes.Add(lane);
                            break;
                        case NoteOffEvent:
                            if (openedSource is null)
                                session.HandleUnopened(globalPosition, () => GetOrCreateChord(globalPosition, track).Notes.Add(lane));
                            else
                            {
                                var length = globalPosition - openedSource.Position;

                                if (length < session.Formatting?.SustainCutoff)
                                    length = 0;

                                openedSource.Notes[lane]!.Length = length;
                                openedNoteSources[track.Difficulty][lane] = null;
                            }
                            break;
                    }
                    break;
            }

            TChord GetOrCreateChord(uint newChordPosition, Track<TChord> track)
            {
                var chord = previousChords[track.Difficulty];

                if (chord is null || chord.Position != newChordPosition)
                    track.Chords.Add(chord = previousChords[track.Difficulty] = CreateChord(newChordPosition));

                return chord;
            }
        }

        protected virtual bool CustomHandle(NoteEvent note) => false;
        protected virtual bool CustomTextHandle(TextEvent text) => false;

        protected abstract TLane ToLane(byte index);
        protected abstract void AddModifier(TChord chord, byte index);
        protected abstract TChord CreateChord(uint position);


        protected Track<TChord> GetOrCreateTrack(Difficulty difficulty) => tracks[(int)difficulty] ??= new() { Difficulty = difficulty };
    }
}
