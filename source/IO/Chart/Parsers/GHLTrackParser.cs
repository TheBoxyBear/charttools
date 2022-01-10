using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class GHLTrackParser : VariableInstrumentTrackParser<GHLChord, GHLInstrumentIdentity>
    {
        public GHLTrackParser(Difficulty difficulty, GHLInstrumentIdentity instrument, ReadingSession session) : base(difficulty, instrument, session) { }

        public override void ApplyResultToSong(Song song)
        {
            var inst = song.GetInstrument(Instrument);

            if (inst is null)
                song.SetInstrument(inst = new(), Instrument);

            ApplyResultToInstrument(inst);
        }

        protected override void HandleNote(Track<GHLChord> track, ref GHLChord chord, uint position, NoteData data, ref bool newChord, out Enum initialModifier)
        {
            // Find the parent chord or create it
            if (chord is null)
                chord = new(position);
            else if (position != chord.Position)
                chord = track.Chords.FirstOrDefault(c => c.Position == position, new(position), out newChord)!;
            else
                newChord = false;

            initialModifier = chord!.Modifier;

            switch (data.NoteIndex)
            {
                // White notes
                case < 3:
                    chord!.Notes.Add(new Note<GHLLane>((GHLLane)(data.NoteIndex + 4)) { Length = data.SustainLength });
                    break;
                // Black 1 and 2
                case < 5:
                    chord!.Notes.Add(new Note<GHLLane>((GHLLane)(data.NoteIndex - 2)) { Length = data.SustainLength });
                    break;
                case 5:
                    chord!.Modifier |= GHLChordModifier.HopoInvert;
                    return;
                case 6:
                    chord!.Modifier |= GHLChordModifier.Tap;
                    return;
                case 7:
                    chord!.Notes.Add(new Note<GHLLane>(GHLLane.Open) { Length = data.SustainLength });
                    break;
                case 8:
                    chord!.Notes.Add(new Note<GHLLane>(GHLLane.Black3) { Length = data.SustainLength });
                    break;
            }
        }
    }
}
