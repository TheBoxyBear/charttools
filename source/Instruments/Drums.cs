using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record Drums : Instrument<DrumsChord>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Drums;
    public override InstrumentType InstrumentType => InstrumentType.Drums;

    internal override InstrumentMapper<DrumsChord> GetMidiMapper(WritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}
