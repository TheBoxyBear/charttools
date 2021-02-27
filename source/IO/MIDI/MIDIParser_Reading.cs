using ChartTools.SystemExtensions;
using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartTools.IO.MIDI
{
    /// <summary>
    /// Provides methods for reading and writing MIDI files
    /// </summary>
    internal static partial class MIDIParser
    {
        private static readonly ReadingSettings readingSettings = new ReadingSettings
        {
            NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
            InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
            NoHeaderChunkPolicy = NoHeaderChunkPolicy.Ignore
        };

        internal static Song ReadSong(string path)
        {
            MidiFile file;
            try { file = MidiFile.Read(path, readingSettings); }
            catch { throw; }

            Song song = new Song();
            Type songType = typeof(Song);
            ChunksCollection chunks = file.Chunks;

            List<Task> tasks = new List<Task>()
            {
                Task.Run(() =>
                {
                    // Global events
                }),
                Task.Run(() =>
                {
                    // Sync track
                }),
                Task.Run(() =>
                {
                    // Drums
                })
            };

            try { song.Metadata = GetMetadata(file); }
            catch { throw; }

            return song;
        }

        internal static Instrument ReadInstrument(string path, Instruments instrument)
        {
            if (instrument == Instruments.Drums)
            {
                try { throw new NotImplementedException(); }
                catch { throw; }
            }
            if (Enum.IsDefined(typeof(GHLInstrument), instrument))
            {
                try { throw new NotImplementedException(); }
                catch { throw; }
            }
            if (Enum.IsDefined(typeof(StandardInstrument), instrument))
            {
                try { throw new NotImplementedException(); }
                catch { throw; }
            }

            throw IOExceptions.GetUndefinedInstrumentException();
        }
        internal static Instrument<DrumsChord> ReadDrums(string path)
        {
            throw new NotImplementedException();
        }
        internal static Instrument<GHLChord> ReadInstrument(string path, GHLInstrument instrument)
        {
            throw new NotImplementedException();
        }
        internal static Instrument<StandardChord> ReadInstrument(string path, StandardInstrument instrument)
        {
            try { return GetInstrument(MidiFile.Read(path, readingSettings).Chunks, instrument); }
            catch { throw; }
        }
        private static Instrument<StandardChord> GetInstrument(ChunksCollection chunks, StandardInstrument instrument)
        {
            if (!Enum.IsDefined(typeof(StandardInstrument), instrument))
                throw new ArgumentException($"Instrument \"{instrument}\" is not defined.");

            Exception noTrackChunkException = CheckTrackChunkPresence(chunks);

            if (noTrackChunkException is not null)
                throw noTrackChunkException;

            try { return GetInstrument(GetSequenceEvents(chunks.OfType<TrackChunk>(), sequenceNames[(Instruments)instrument]), GetStandardTrack); }
            catch { throw; }
        }
        private static Instrument<TChord> GetInstrument<TChord>(IEnumerable<MidiEvent> events, Func<IEnumerable<MidiEvent>, Difficulty, Track<TChord>> getTrack) where TChord : Chord
        {
            Instrument<TChord> inst = new Instrument<TChord>();
            Difficulty[] difficulties = EnumExtensions.GetValues<Difficulty>().ToArray();
            Type instrumentType = typeof(Instrument<TChord>);
            Task<Track<TChord>>[] tasks = difficulties.Select(d => Task.Run(() =>
            {
                try { return getTrack(events, d); }
                catch { throw; }
            })).ToArray();

            List<LocalEvent> localEvents = new List<LocalEvent>();

            try { localEvents = GetLocalEvents(events); }
            catch { throw; }

            for (int i = 0; i < 4; i++)
            {
                Task<Track<TChord>> task = tasks[i];

                try { task.Wait(); }
                catch { throw; }

                task.Result.LocalEvents = localEvents;
                instrumentType.GetProperty(((Difficulty)i).ToString()).SetValue(inst, task.Result);

                task.Dispose();
            }

            return difficulties.Select(d => inst.GetTrack(d)).All(t => t is null) ? null : inst;
        }

        private static Track<StandardChord> GetStandardTrack(IEnumerable<MidiEvent> events, Difficulty difficulty)
        {
            throw new NotImplementedException();
        }

        private static List<LocalEvent> GetLocalEvents(IEnumerable<MidiEvent> events)
        {
            foreach (MidiEvent e in events)
            {

            }

            throw new NotImplementedException();
        }

        internal static Metadata ReadMetadata(string path)
        {
            try { return GetMetadata(MidiFile.Read(path, readingSettings)); }
            catch { throw; }
        }
        private static Metadata GetMetadata(MidiFile file)
        {
            Exception noTrackChunkException = CheckTrackChunkPresence(file.Chunks);

            if (noTrackChunkException is not null)
                throw noTrackChunkException;

            string title;

            try { title = ((file.Chunks[0] as TrackChunk).Events[0] as SequenceTrackNameEvent).Text; }
            catch { throw; }

            return file.TimeDivision is TicksPerQuarterNoteTimeDivision division
                ? new Metadata { Resolution = (ushort)division.TicksPerQuarterNote, Title = title }
                : throw new FormatException("Cannot get resolution from the file.");
        }

        internal static SyncTrack ReadSyncTrack(string path)
        {
            try { return GetSyncTrack(MidiFile.Read(path, readingSettings).Chunks); }
            catch { throw; }
        }
        private static SyncTrack GetSyncTrack(ChunksCollection chunks)
        {
            Exception noTrackChunkException = CheckTrackChunkPresence(chunks);

            if (noTrackChunkException is not null)
                throw noTrackChunkException;

            SyncTrack syncTrack = new SyncTrack();

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

            if (chunk is not null)
                foreach (MidiEvent ev in chunk.Events.Where(e => e is not SequenceTrackNameEvent))
                    yield return ev;
        }

        private static Exception CheckTrackChunkPresence(ChunksCollection chunks) => chunks.Count == 0
                ? new FormatException("File has no chunks.")
                : chunks.Count(c => c is TrackChunk) == 0 ? new FormatException("File has no track chunks.") : null;
    }
}
