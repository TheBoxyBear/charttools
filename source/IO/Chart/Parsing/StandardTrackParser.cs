using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsing
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
            switch (data.Index)
            {
                // Colored note
                case < 5:
                    AddNote(new((StandardLane)(data.Index + 1)) { Length = data.SustainLength });
                    break;
                case 5:
                    AddModifier(StandardChordModifiers.HopoInvert);
                    return;
                case 6:
                    AddModifier(StandardChordModifiers.Tap);
                    return;
                case 7:
                    AddNote(new(StandardLane.Open) { Length = data.SustainLength });
                    break;
            }

            void AddNote(LaneNote<StandardLane> note) => HandleAddNote(note, () => chord.Notes.Add(note));
            void AddModifier(StandardChordModifiers modifier) => HandleAddModifier(chord.Modifiers, modifier, () => chord.Modifiers |= modifier);
        }
    }
}
