using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

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

        protected override void HandleNoteEntry(StandardChord chord, NoteData data)
        {
            switch (data.NoteIndex)
            {
                // Colored note
                case < 5:
                    chord.Notes.Add(new Note<StandardLane>((StandardLane)(data.NoteIndex + 1)) { Length = data.SustainLength });
                    break;
                case 5:
                    chord.Modifier |= StandardChordModifier.HopoInvert;
                    return;
                case 6:
                    chord.Modifier |= StandardChordModifier.Tap;
                    return;
                case 7:
                    chord.Notes.Add(new Note<StandardLane>(StandardLane.Open) { Length = data.SustainLength });
                    break;
            }

            void AddNote(Note<StandardLane> note) => HandleAddNote(note, () => chord.Notes.Add(note));
            void AddModifier(StandardChordModifier modifier) => HandleAddModifier(chord.Modifier, modifier, () => chord.Modifier |= modifier);
        }

        protected override StandardChord CreateChord(uint position) => new(position);
    }
}
