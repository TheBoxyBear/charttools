namespace ChartTools.Lyrics;

public record Vocals : Instrument<Phrase>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Vocals;

    internal override InstrumentMapper<Phrase> GetMidiMapper(MidiWritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
}