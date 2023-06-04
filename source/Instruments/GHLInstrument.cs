using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record GHLInstrument : Instrument<GHLChord>
{
    public new GHLInstrumentIdentity InstrumentIdentity { get; init; } = GHLInstrumentIdentity.Guitar;
    public override InstrumentType InstrumentType => InstrumentType.GHL;

    public GHLInstrument() { }
    public GHLInstrument(GHLInstrumentIdentity identity)
    {
        Validator.ValidateEnum(identity);
        InstrumentIdentity = identity;
    }

    protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;

    internal override InstrumentMapper<GHLChord> GetMidiMapper(WritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}
