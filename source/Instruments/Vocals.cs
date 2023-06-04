using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.Lyrics;

namespace ChartTools;

public record Vocals : Instrument<Phrase>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Vocals;
    public override InstrumentType InstrumentType => InstrumentType.Vocals;

    internal override InstrumentMapper<Phrase> GetMidiMapper(WritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}
