namespace ChartTools;

public record StandardInstrument : Instrument<StandardChord>
{
    public new StandardInstrumentIdentity InstrumentIdentity { get; init; }


    /// <summary>
    /// Format of lead guitar and bass. Not applicable to other instruments.
    /// </summary>
    public MidiInstrumentOrigin MidiOrigin
    {
        get => midiOrigin;
        set
        {
            if (value is MidiInstrumentOrigin.GuitarHero1 && InstrumentIdentity is not StandardInstrumentIdentity.LeadGuitar)
                throw new ArgumentException($"{InstrumentIdentity} is not supported by Guitar Hero 1.", nameof(value));

            midiOrigin = value;
        }
    }
    private MidiInstrumentOrigin midiOrigin;

    public StandardInstrument() { }
    public StandardInstrument(StandardInstrumentIdentity identity) => InstrumentIdentity = identity;

    protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;
}
