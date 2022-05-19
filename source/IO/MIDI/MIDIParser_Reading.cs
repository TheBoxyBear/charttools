using ChartTools.SystemExtensions.Linq;
using ChartTools.Collections.Alternating;
using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChartTools.IO.Configuration;
using ChartTools.Events;
using ChartTools.Exceptions;

namespace ChartTools.IO.MIDI
{
    /// <summary>
    /// Provides methods for reading and writing MIDI files
    /// </summary>
    internal static partial class MIDIParser
    {
        /// <summary>
        /// Settings to customize the initial parsing of the file by DryWetMidi
        /// </summary>
        private static ReadingSettings ReadingSettings { get; set; } 

        /// <summary>
        /// Reads a MIDI file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="midiConfig">Parameters used to interpret the file</param>
        public static Song ReadSong(string path, ReadingConfiguration midiConfig)
        {
            if (midiConfig is null)
                throw new ArgumentNullException(nameof(midiConfig));

            MidiFile file = MidiFile.Read(path, ReadingSettings);

            Song song = new();
            Type songType = typeof(Song);
            ChunksCollection chunks = file.Chunks;

            // Tasks for global events, sync track and drums
            List<Task> tasks = new();
            //{
            //    Task.Run(() => song.GlobalEvents = GetGlobalEvents(chunks)),
            //    Task.Run(() => song.SyncTrack = GetSyncTrack(chunks)),
            //    Task.Run(() => song.Drums = GetDrums(chunks, midiConfig))
            //};

            //// Tasks for each GHL instrument
            //foreach (GHLInstrument inst in Enum.GetValues<GHLInstrument>())
            //    tasks.Add(Task.Run(() => songType.GetProperty(inst.ToString())!.SetValue(song, GetInstrument(chunks, inst, midiConfig))));
            //// Tasks for each standard instrument
            //foreach (StandardInstrument inst in Enum.GetValues<StandardInstrument>())
            tasks.Add(Task.Run(() => songType.GetProperty(StandardInstrumentIdentity.LeadGuitar.ToString())!.SetValue(song, GetInstrument(chunks, StandardInstrumentIdentity.LeadGuitar, midiConfig))));

            foreach (Task task in tasks)
            {
                task.Wait();
                task.Dispose();
            }

            return song;
        }

        public static Instrument? ReadInstrument(string path, InstrumentIdentity instrument, ReadingConfiguration midiConfig)
        {
            if (midiConfig is null)
                throw new ArgumentNullException(nameof(midiConfig));

            MidiFile file = MidiFile.Read(path, ReadingSettings);

            if (instrument == InstrumentIdentity.Drums)
                return GetDrums(file.Chunks, midiConfig);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return GetInstrument(file.Chunks, (GHLInstrumentIdentity)instrument, midiConfig);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return GetInstrument(file.Chunks, (StandardInstrumentIdentity)instrument, midiConfig);

            throw new UndefinedEnumException(instrument);
        }

        public static Instrument<DrumsChord>? ReadDrums(string path, ReadingConfiguration midiConfig) => midiConfig is null
            ? throw new ArgumentNullException(nameof(midiConfig))
            : GetDrums(MidiFile.Read(path, ReadingSettings).Chunks, midiConfig);
        public static Instrument<GHLChord>? ReadInstrument(string path, GHLInstrumentIdentity instrument, ReadingConfiguration midiConfig) => midiConfig is null
            ? throw new ArgumentNullException(nameof(midiConfig))
            : GetInstrument(MidiFile.Read(path, ReadingSettings).Chunks, instrument, midiConfig);
        public static Instrument<StandardChord>? ReadInstrument(string path, StandardInstrumentIdentity instrument, ReadingConfiguration midiConfig) => midiConfig is null
            ? throw new ArgumentNullException(nameof(midiConfig))
            : GetInstrument(MidiFile.Read(path, ReadingSettings).Chunks, instrument, midiConfig);

        private static Instrument<DrumsChord>? GetDrums(ChunksCollection chunks, ReadingConfiguration midiConfig) => CheckTrackChunkPresence(chunks, out Exception? e)
            ? GetInstrument(GetSequenceEvents(chunks.OfType<TrackChunk>(), sequenceNames[InstrumentIdentity.Drums]), GetDrumsChords, midiConfig)
            : throw e!;
        private static Instrument<GHLChord>? GetInstrument(ChunksCollection chunks, GHLInstrumentIdentity instrument, ReadingConfiguration midiConfig)
        {
            Validator.ValidateEnum(instrument);

            if (!CheckTrackChunkPresence(chunks, out Exception? e))
                throw e!;

            return GetInstrument(GetSequenceEvents(chunks.OfType<TrackChunk>(), sequenceNames[(InstrumentIdentity)instrument]), GetGHLChords, midiConfig);
        }
        private static Instrument<StandardChord>? GetInstrument(ChunksCollection chunks, StandardInstrumentIdentity instrument, ReadingConfiguration midiConfig)
        {
            Validator.ValidateEnum(instrument);

            if (!CheckTrackChunkPresence(chunks, out Exception? e))
                throw e!;

            return GetInstrument(GetSequenceEvents(chunks.OfType<TrackChunk>(), sequenceNames[(InstrumentIdentity)instrument]), GetStandardChords, midiConfig);
        }

        private static Instrument<TChord>? GetInstrument<TChord>(IEnumerable<MidiEvent> events, Func<IEnumerable<MidiEvent>, Difficulty, ReadingConfiguration, UniqueTrackObjectCollection<TChord>> getChords, ReadingConfiguration config) where TChord : Chord
        {
            Instrument<TChord> instrument = new();
            var difficulties = Enum.GetValues<Difficulty>().ToArray();
            var tasks = new Task<UniqueTrackObjectCollection<TChord>>[] { Task.Run(() => getChords(events, Difficulty.Expert, config)) }; /*difficulties.Select(d => Task.Run(() => getTrack(events, d, config))).ToArray();*/

            (List<LocalEvent> localEvents, List<SpecialPhrase> starPower) = GetLocalEventsStarPower(events, config);
            bool eventsOrStarPower = localEvents.Count > 0 || starPower.Count > 0;

            Task.WaitAll(tasks);

            if (tasks.Any(t => t.Result is not null))
            {
                Type instrumentType = typeof(Instrument<TChord>);

                for (int i = 0; i < difficulties.Length; i++)
                {
                    UniqueTrackObjectCollection<TChord> chords = tasks[i].Result;

                    if (eventsOrStarPower || chords.Count > 0)
                    {
                        Track<TChord> track = new();
                        track.LocalEvents!.AddRange(localEvents);
                        track.SpecialPhrases.AddRange(starPower);

                        instrumentType.GetProperty(difficulties[i].ToString())!.SetValue(instrument, track);
                    }
                }

                return instrument;
            }

            return null;
        }

        private static UniqueTrackObjectCollection<DrumsChord> GetDrumsChords(IEnumerable<MidiEvent> events, Difficulty difficulty, ReadingConfiguration midiConfig)
        {
            throw new NotImplementedException();
        }
        private static UniqueTrackObjectCollection<GHLChord> GetGHLChords(IEnumerable<MidiEvent> events, Difficulty difficulty, ReadingConfiguration midiConfig)
        {
            throw new NotImplementedException();
        }
        private static UniqueTrackObjectCollection<StandardChord> GetStandardChords(IEnumerable<MidiEvent> events, Difficulty difficulty, ReadingConfiguration midiConfig)
        {
            NoteMode mode = NoteMode.Regular;
            Note<StandardLane>[] sustainedNotes = new Note<StandardLane>[6]; // Stores references notes generated from a NoteON event until they are closed by a NoteOff

            Predicate<byte> NoteMatchDifficulty = difficulty switch
            {
                Difficulty.Easy => n => n / 10 == 6,
                Difficulty.Medium => n => n / 10 == 7,
                Difficulty.Hard => n => n / 10 == 8,
                Difficulty.Expert => n => n is > 89 and < 111,
                _ => throw new UndefinedEnumException(difficulty)
            };

            byte difficultyNoteIndexOffset = (byte)(10 * ((int)difficulty + 6) + (int)difficulty * 2);
            byte GetNoteIndex(byte noteNumber) => (byte)(noteNumber - difficultyNoteIndexOffset);

            UniqueTrackObjectCollection<StandardChord> chords = new();
            StandardChord? chord = null;

            Dictionary<StandardLane, StandardChord?> sustainOrigins =
                new(from note in Enum.GetValues<StandardLane>()
                    select new KeyValuePair<StandardLane, StandardChord?>(note, null));

            bool newChord = true;

            void GetParentChord(NoteEvent e, uint pos)
            {
                // Find the parent chord or create it
                if (chord is null)
                    chord = new((uint)e.DeltaTime);
                else if (pos != chord.Position)
                    chord = chords.FirstOrDefault(c => c.Position == pos, new(pos), out newChord);
                else
                    newChord = false;
            }

            foreach (MidiEvent e in events)
                switch (e)
                {
                    case NormalSysExEvent normalSysEvent:
                        if (normalSysEvent.Data[4] != (byte)difficulty)
                            continue;
                        break;
                    case NoteOnEvent noteOnEvent:
                        if (!NoteMatchDifficulty(noteOnEvent.NoteNumber))
                            continue;

                        uint position = (uint)noteOnEvent.DeltaTime;
                        StandardLane noteEnum = (StandardLane)GetNoteIndex(noteOnEvent.NoteNumber);

                        GetParentChord(noteOnEvent, position);
                        sustainOrigins[noteEnum] = chord;

                        chord!.Notes.Add(new Note<StandardLane>(noteEnum));

                        if (newChord)
                            chords.Add(chord);

                        break;
                    case NoteOffEvent noteOffEvent:
                        if (!NoteMatchDifficulty(noteOffEvent.NoteNumber))
                            continue;

                        position = (uint)noteOffEvent.DeltaTime;
                        GetParentChord(noteOffEvent, position);

                        noteEnum = (StandardLane)GetNoteIndex(noteOffEvent.NoteNumber);

                        if (sustainOrigins.ContainsKey(noteEnum))
                        {
                            StandardChord? ch = sustainOrigins[noteEnum];
                            Note<StandardLane>? note = ch?.Notes[noteEnum];

                            if (note is not null)
                                note.Length = position - ch!.Position;
                        }
                        break;
                }

            return chords;
        }
        private static Track<TChord>? GetTrack<TChord>(IEnumerable<MidiEvent> events, Difficulty difficulty, ReadingConfiguration midiConfig) where TChord : Chord
        {
            throw new NotImplementedException();
        }

        public static List<GlobalEvent> ReadGlobalEvents(string path) => GetGlobalEvents(MidiFile.Read(path, ReadingSettings).Chunks);
        private static List<GlobalEvent> GetGlobalEvents(ChunksCollection chunks)
        {
            if (!CheckTrackChunkPresence(chunks, out Exception? e))
                throw e!;

            // Get the events in the global events track and vocal track, alternating between the two by taking the lowest DeltaTime
            return new List<GlobalEvent>(new OrderedAlternatingEnumerable<long, MidiEvent>(e => e.DeltaTime, GetSequenceEvents(chunks.OfType<TrackChunk>(), globalEventSequenceName), GetSequenceEvents(chunks.OfType<TrackChunk>(), lyricsSequenceName)).Select(e => new GlobalEvent((uint)e.DeltaTime, e switch
            {
                // For each event, select a new GlobalEvent with DeltaTime as Position and EventData based on the type of MIDIEvent
                TextEvent textEvent => textEvent.Text,
                NoteOnEvent => EventTypeHelper.Global.PhraseStart,
                NoteOffEvent => EventTypeHelper.Global.PhraseEnd,
                LyricEvent lyricEvent => $"{EventTypeHelper.Global.Lyric} {lyricEvent.Text}",
                _ => e.EventType.ToString()
            })).Where(e => e is not null));
        }

        private static (List<LocalEvent> localEvents, List<SpecialPhrase> starPower) GetLocalEventsStarPower(IEnumerable<MidiEvent> events, ReadingConfiguration midiConfig)
        {
            List<LocalEvent> localEvents = new();
            List<SpecialPhrase> starPower = new();

            bool unfinishedSolo = false;
            SpecialPhrase? sp = null;

            const int spNoteNumber = 116;

            // First pass checking star power defined as NoteEvent
            foreach (NoteEvent e in events.OfType<NoteEvent>())
                switch (e)
                {
                    // Starpower start
                    case NoteOnEvent noteOnEvent when noteOnEvent.NoteNumber == spNoteNumber && !unfinishedSolo:
                        sp = new SpecialPhrase((uint)e.DeltaTime, SpecialPhraseType.StarPowerGain);
                        unfinishedSolo = true;
                        break;
                    case NoteOffEvent noteOffEvent when sp is not null && noteOffEvent.NoteNumber == spNoteNumber && unfinishedSolo:
                        sp.Length = (uint)e.DeltaTime - sp.Position;
                        unfinishedSolo = false;
                        break;
                }

            // File must be interpreted as old MIDI where solo and soloend events define star power
            bool convertEvent = midiConfig.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert && starPower.Count == 0;

            // Second pass checking text events
            foreach (TextEvent e in events.OfType<TextEvent>())
            {
                if (convertEvent)
                {
                    switch (e.Text)
                    {
                        // Star power start
                        case "solo" when !unfinishedSolo:
                            sp = new SpecialPhrase((uint)e.DeltaTime, SpecialPhraseType.StarPowerGain);
                            unfinishedSolo = true;
                            break;
                        // Star power end
                        case "soloend" when unfinishedSolo:
                            sp!.Length = (uint)e.DeltaTime - sp.Position;
                            starPower.Add(sp);
                            unfinishedSolo = false;
                            break;
                        default:
                            localEvents.Add(new((uint)e.DeltaTime, e.Text));
                            break;
                    }
                }
                else
                    localEvents.Add(new((uint)e.DeltaTime, e.Text));
            }

            return (localEvents, starPower);
        }

        public static Metadata ReadMetadata(string path) => GetMetadata(MidiFile.Read(path, ReadingSettings));
        private static Metadata GetMetadata(MidiFile file)
        {
            if (!CheckTrackChunkPresence(file.Chunks, out Exception? e))
                throw e!;

            string title = ((file.Chunks[0] as TrackChunk)!.Events[0] as SequenceTrackNameEvent)!.Text;

            return file.TimeDivision is TicksPerQuarterNoteTimeDivision division
                ? new() { Formatting = new() { Resolution = (ushort)division.TicksPerQuarterNote }, Title = title }
                : throw new FormatException("Cannot get resolution from the file.");
        }

        public static SyncTrack ReadSyncTrack(string path) => GetSyncTrack(MidiFile.Read(path, ReadingSettings).Chunks);
        private static SyncTrack GetSyncTrack(ChunksCollection chunks)
        {
            if (!CheckTrackChunkPresence(chunks, out Exception? ex))
                throw ex!;

            SyncTrack syncTrack = new();

            foreach (MidiEvent e in chunks.OfType<TrackChunk>().First().Events)
                switch (e)
                {
                    case TimeSignatureEvent ts:
                        syncTrack.TimeSignatures.Add(new TimeSignature((uint)ts.DeltaTime, ts.Numerator, ts.Denominator));
                        break;
                    case SetTempoEvent tempo:
                        syncTrack.Tempo.Add(new Tempo((uint)tempo.DeltaTime, 60000000 / tempo.MicrosecondsPerQuarterNote));
                        break;
                }

            return syncTrack;
        }

        private static IEnumerable<MidiEvent> GetSequenceEvents(IEnumerable<TrackChunk> chunks, string sequenceName)
        {
            // Get the chunk matching the desired sequence name
            TrackChunk chunk = chunks.First(c => c.Events.OfType<SequenceTrackNameEvent>().First().Text == sequenceName);

            return chunk is null
                ? Enumerable.Empty<MidiEvent>()
                : chunk.Events.Where(e => e is not SequenceTrackNameEvent);
        }

        /// <summary>
        /// Checks if a <see cref="ChunksCollection"/> contains any <see cref="TrackChunk"/>.
        /// </summary>
        /// <param name="chunks">Chunks to check</param>
        /// <param name="ex">Exception to throw if returned <see langword="false"/></param>
        /// <returns><see langword="true"/> if the collection contains at least one <see cref="TrackChunk"/></returns>
        private static bool CheckTrackChunkPresence(ChunksCollection chunks, out Exception? ex)
        {
            if (chunks.Count == 0)
            {
                ex = new FormatException("File has no chunks.");
                return false;
            }

            if (chunks.Any(chunks => chunks is TrackChunk))
            {
                ex = null;
                return true;
            }

            ex = new FormatException("File has no track chunks.");
            return false;
        }
    }
}
