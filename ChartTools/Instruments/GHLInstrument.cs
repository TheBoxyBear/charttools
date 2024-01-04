using ChartTools.Animations;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record GHLInstrument : Instrument<GHLChord>
{
    public new GHLInstrumentIdentity InstrumentIdentity { get; init; }

    public GHLInstrument() { }
    public GHLInstrument(GHLInstrumentIdentity identity) => InstrumentIdentity = identity;

    protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;

    internal override IInstrumentWriteMapper<GHLChord> GetMidiMapper(MidiWritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}
