using ChartTools.IO.Midi.Configuration.Sessions;

namespace ChartTools.IO.Midi.Parsing;

internal abstract class InstrumentParser<TChord>(MidiReadingSession session) : MidiParser(session) where TChord : IChord, new()
{
    protected readonly Track<TChord>[] tracks = new Track<TChord>[4];

    public override Instrument<TChord> Result => GetResult(GetInstrument());

    protected abstract Instrument<TChord> GetInstrument();

    protected override void FinaliseParse()
    {
        foreach (var track in tracks)
            GetInstrument().SetTrack(track);

        base.FinaliseParse();
    }

    protected uint GetSustain(uint start, uint end)
    {
        var length = end - start;
        return length < session.Formatting?.SustainCutoff ? 0 : length;
    }
}