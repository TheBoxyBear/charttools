using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing;

internal abstract class InstrumentParser<TChord> : MidiParser where TChord : IChord, new()
{
    protected readonly Track<TChord>[] tracks = new Track<TChord>[4];

    public override Instrument<TChord> Result => GetResult(GetInstrument());

    protected readonly InstrumentMapper<TChord> mapper;

    protected InstrumentParser(InstrumentMapper<TChord> mapper, ReadingSession session) : base(session) => this.mapper = mapper;

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