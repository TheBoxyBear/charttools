using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Parsing
{
    internal class GHLTrackParser : VariableInstrumentTrackParser<GHLChord, GHLInstrumentIdentity>
    {
        public GHLTrackParser(Difficulty difficulty, GHLInstrumentIdentity instrument, ReadingSession session, string header) : base(difficulty, instrument, session, header) { }

        public override void ApplyToSong(Song song)
        {
            var inst = song.Instruments.Get(Instrument);

            if (inst is null)
                song.Instruments.Set(inst = new() { InstrumentIdentity = (InstrumentIdentity)Instrument });

            ApplyToInstrument(inst);
        }

        protected override void HandleNoteEntry(GHLChord chord, NoteData data)
        {
            switch (data.NoteIndex)
            {
                // White notes
                case < 3:
                    AddNote(new Note<GHLLane>((GHLLane)(data.NoteIndex + 4)) { Length = data.SustainLength });
                    break;
                // Black 1 and 2
                case < 5:
                    AddNote(new Note<GHLLane>((GHLLane)(data.NoteIndex - 2)) { Length = data.SustainLength });
                    break;
                case 5:
                    AddModifier(GHLChordModifier.HopoInvert);
                    return;
                case 6:
                    AddModifier(GHLChordModifier.Tap);
                    return;
                case 7:
                    AddNote(new Note<GHLLane>(GHLLane.Open) { Length = data.SustainLength });
                    break;
                case 8:
                    AddNote(new Note<GHLLane>(GHLLane.Black3) { Length = data.SustainLength });
                    break;
            }

            void AddNote(Note<GHLLane> note) => HandleAddNote(note, () => chord.Notes.Add(note));
            void AddModifier(GHLChordModifier modifier) => HandleAddModifier(chord.Modifier, modifier, () => chord.Modifier |= modifier);
        }

        protected override GHLChord CreateChord(uint position) => new(position);
    }
}
