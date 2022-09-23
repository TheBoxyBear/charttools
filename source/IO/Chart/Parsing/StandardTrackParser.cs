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
                    chord.Notes.Add(new LaneNote<StandardLane>((StandardLane)(data.Index + 1)) { Sustain = data.SustainLength });
                    break;
                case 5:
                    chord.Modifiers |= StandardChordModifiers.HopoInvert;
                    return;
                case 6:
                    chord.Modifiers |= StandardChordModifiers.Tap;
                    return;
                case 7:
                    chord.Notes.Add(new LaneNote<StandardLane>(StandardLane.Open) { Sustain = data.SustainLength });
                    break;
            }

            void AddNote(LaneNote<StandardLane> note) => HandleAddNote(note, () => chord.Notes.Add(note));
            void AddModifier(StandardChordModifiers modifier) => HandleAddModifier(chord.Modifiers, modifier, () => chord.Modifiers |= modifier);
        }
    }
}
