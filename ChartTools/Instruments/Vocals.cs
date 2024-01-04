using ChartTools.Animations;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.Lyrics;

namespace ChartTools;

public record Vocals : Instrument<Phrase>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Vocals;

    internal override InstrumentMapper<Phrase> GetMidiMapper(MidiWritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}