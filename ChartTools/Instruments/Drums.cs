using ChartTools.Animations;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record Drums : Instrument<DrumsChord>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Drums;

    internal override InstrumentMapper<DrumsChord> GetMidiMapper(MidiWritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}