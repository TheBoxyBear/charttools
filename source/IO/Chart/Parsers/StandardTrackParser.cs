using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class StandardTrackParser : VariableInstrumentTrackParser<StandardChord, StandardInstrumentIdentity>
    {
        public StandardTrackParser(Difficulty difficulty, StandardInstrumentIdentity instrument, ReadingSession session, string header) : base(difficulty, instrument, session, header) { }

        public override void ApplyToSong(Song song)
        {
            var inst = song.Instruments.Get(Instrument);

            if (inst is null)
                song.Instruments.Set(inst = new() { InstrumentIdentity = (InstrumentIdentity)Instrument });

            ApplyToInstrument(inst);
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
