using ChartTools.IO.Chart.Entries;
using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class StandardTrackParser : VariableInstrumentTrackParser<StandardChord, StandardInstrumentIdentity>
    {
        public StandardTrackParser(Difficulty difficulty, StandardInstrumentIdentity instrument) : base(difficulty, instrument) { }

        public override void ApplyResultToSong(Song song)
        {
            var inst = song.GetInstrument(Instrument);
            var instrumentExists = inst is not null;

            if (!instrumentExists)
                inst = new();

            ApplyResultToInstrument(inst!);

            if (!instrumentExists)
                song.SetInstrument(inst!, Instrument);
        }

        protected override void HandleNote(Track<StandardChord> track, ref StandardChord chord, uint position, NoteData data, ref bool newChord, out Enum initialModifier)
        {
            // Find the parent chord or create it
            if (chord is null)
                chord = new(position);
            else if (newChord = position != chord!.Position)
                chord = track.Chords.Find(c => c.Position == position) ?? new(position);

            initialModifier = chord!.Modifier;

            switch (data.NoteIndex)
            {
                // Colored note
                case < 5:
                    chord!.Notes.Add(new Note<StandardLane>((StandardLane)(data.NoteIndex + 1)) { Length = data.SustainLength });
                    break;
                case 5:
                    chord!.Modifier |= StandardChordModifier.HopoInvert;
                    return;
                case 6:
                    chord!.Modifier |= StandardChordModifier.Tap;
                    return;
                case 7:
                    chord!.Notes.Add(new Note<StandardLane>(StandardLane.Open) { Length = data.SustainLength });
                    break;
            }
        }
    }
}
