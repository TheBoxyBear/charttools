using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ChartTools.SystemExtensions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.MIDI
{
    /// <summary>
    /// Provides methods for reading and writing MIDI files
    /// </summary>
    internal static partial class MIDIParser
    {
        internal static Song ReadSong(string path)
        {
            MidiFile file;
            try { file = MidiFile.Read(path); }
            catch { throw; }

            Song song = new Song();
            Type songType = typeof(Song);

            List<Task> tasks = new List<Task>()
            {
                
            };

            try { song.Metadata = GetMetadata(file.Chunks); }
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
            throw new NotImplementedException();
        }
        private static Instrument<TChord> GetInstrument<TChord>(EventsCollection events, Func<EventsCollection, Difficulty, Track<TChord>> getTrack, List<LocalEvent> localEvents = null) where TChord : Chord
        {
            Instrument<TChord> inst = new Instrument<TChord>();

            if (localEvents is not null)
                foreach (Difficulty diff in EnumExtensions.GetValues<Difficulty>())
                    inst.GetTrack(diff).LocalEvents = localEvents;

            throw new NotImplementedException();
        }

        internal static Metadata ReadMetadata(string path)
        {
            throw new NotImplementedException();
        }
        private static Metadata GetMetadata(ChunksCollection chunks)
        {
            string title = string.Empty;

            try { title = ((chunks[0] as TrackChunk).Events[0] as SequenceTrackNameEvent).Text; }
            catch { throw; }

            return new Metadata { Title = title };
        }

        private static IEnumerable<MidiEvent> GetSequenceEvents(ChunksCollection chunks, string sequenceName)
        {
            // Get the chunk matching the desired sequence name
            TrackChunk chunk = chunks.OfType<TrackChunk>().FirstOrDefault(c => c.Events.OfType<SequenceTrackNameEvent>().First().Text == sequenceName);

            if (chunk is not null)
                foreach (MidiEvent ev in chunk.Events.Where(e => e is not SequenceTrackNameEvent))
                    yield return ev;
        }
    }
}
