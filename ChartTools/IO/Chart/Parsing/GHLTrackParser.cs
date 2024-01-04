using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Parsing;

internal class GHLTrackParser(Difficulty difficulty, GHLInstrumentIdentity instrument, ChartReadingSession session, string header)
    : VariableInstrumentTrackParser<GHLChord, GHLInstrumentIdentity>(difficulty, instrument, session, header)
{
    public override void ApplyToSong(Song song)
    {
        var inst = song.Instruments.Get(Instrument);

        if (inst is null)
            song.Instruments.Set(inst = new(Instrument));

        ApplyToInstrument(inst);
    }

    protected override void HandleNoteEntry(GHLChord chord, NoteData data)
    {
        switch (data.Index)
        {
            // White notes
            case < 3:
                AddNote(new LaneNote<GHLLane>((GHLLane)(data.Index + 4)) { Sustain = data.SustainLength });
                break;
            // Black 1 and 2
            case < 5:
                AddNote(new LaneNote<GHLLane>((GHLLane)(data.Index - 2)) { Sustain = data.SustainLength });
                break;
            case 5:
                AddModifier(GHLChordModifiers.HopoInvert);
                return;
            case 6:
                AddModifier(GHLChordModifiers.Tap);
                return;
            case 7:
                AddNote(new LaneNote<GHLLane>(GHLLane.Open) { Sustain = data.SustainLength });
                break;
            case 8:
                AddNote(new LaneNote<GHLLane>(GHLLane.Black3) { Sustain = data.SustainLength });
                break;
        }

        void AddNote(LaneNote<GHLLane> note) => HandleAddNote(note, () => chord.Notes.Add(note));
        void AddModifier(GHLChordModifiers modifier) => HandleAddModifier(chord.Modifiers, modifier, () => chord.Modifiers |= modifier);
    }
}
