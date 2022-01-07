using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Tools.Scaling
{
    public static class TempoRescaler
    {
        public static void Rescale(this Note note, float scale) => note.Length = (uint)(note.Length * scale);
        public static void Rescale(this TrackObject trackObject, float scale) => trackObject.Position = (uint)(trackObject.Position * scale);
        public static void Rescale(this SpecicalPhrase starPower, float scale)
        {
            starPower.Position = (uint)(starPower.Position * scale);
            starPower.Length = (uint)(starPower.Length * scale);
        }
        public static void Rescale(this Chord chord, float scale)
        {
            chord.Position = (uint)(chord.Position * scale);

            foreach (var note in chord.Notes)
                note.Rescale(scale);
        }
        public static void Rescale(this Track track, float scale)
        {
            foreach (Chord chord in track.Chords)
                Rescale(chord, scale);

            if (track.LocalEvents is not null)
                foreach (var e in track.LocalEvents)
                    e.Rescale(scale);
        }
        public static void Rescale(this Instrument instrument, float scale)
        {
            foreach (var track in instrument.GetTracks().NonNull())
                track.Rescale(scale);
        }
        public static void Rescale(this SyncTrack syncTrack, float scale)
        {
            foreach (var tempo in syncTrack.Tempo)
            {
                tempo.Position = (uint)(tempo.Position * scale);
                tempo.Value *= scale;
            }
            foreach (var signature in syncTrack.TimeSignatures)
                signature.Rescale(scale);
        }
        public static void Rescale(this Song song, float scale)
        {
            foreach (var instrument in song.GetInstruments().NonNull())
                instrument.Rescale(scale);

            if (song.SyncTrack is not null)
                song.SyncTrack.Rescale(scale);

            if (song.GlobalEvents is not null)
                foreach (var e in song.GlobalEvents)
                    e.Rescale(scale);
        }
    }
}
