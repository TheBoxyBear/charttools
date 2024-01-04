using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record StandardInstrument : Instrument<StandardChord>
{
    public new StandardInstrumentIdentity InstrumentIdentity { get; init; } = StandardInstrumentIdentity.LeadGuitar;
    public override InstrumentType InstrumentType => InstrumentType.Standard;

    /// <summary>
    /// Format of lead guitar and bass. Not applicable to other instruments.
    /// </summary>
    public MidiInstrumentOrigin MidiOrigin
    {
        get => midiOrigin;
        set
        {
            Validator.ValidateEnum(value);

            if (InstrumentIdentity is StandardInstrumentIdentity.LeadGuitar)
            {
                if (value.HasFlag(MidiInstrumentOrigin.GuitarHero1))
                    Error("Guitar Hero 1");
            }
            else if (value.HasFlag(MidiInstrumentOrigin.GuitarHero2) && InstrumentIdentity is not StandardInstrumentIdentity.RhythmGuitar or StandardInstrumentIdentity.CoopGuitar or StandardInstrumentIdentity.Bass)
                Error("Guitar Hero 2");

            void Error(string origin) => throw new ArgumentException($"{InstrumentIdentity} is not supported by {origin}.", nameof(value));

            midiOrigin = value;
        }
    }
    private MidiInstrumentOrigin midiOrigin;

    public StandardInstrument() { }
    public StandardInstrument(StandardInstrumentIdentity identity)
    {
        Validator.ValidateEnum(identity);
        InstrumentIdentity = identity;
    }

    protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;

    internal override InstrumentMapper<StandardChord> GetMidiMapper(WritingSession session, AnimationSet animations)
    {
        var format = MidiOrigin;

        if (MidiOrigin.HasFlag(MidiInstrumentOrigin.Unknown))
        {
            var newFormat = session.UncertainFormatProcedure(InstrumentIdentity, format);

            if (newFormat is MidiInstrumentOrigin.GuitarHero1 && InstrumentIdentity is not StandardInstrumentIdentity.LeadGuitar)
                throw new NotSupportedException($"Guitar Hero 1 does not support instrument {InstrumentIdentity}");

            format = newFormat;
        }

        if (format is MidiInstrumentOrigin.GuitarHero1)
            return new GHGemsMapper(session, animations.Vocals);

        if (InstrumentIdentity is StandardInstrumentIdentity.LeadGuitar or StandardInstrumentIdentity.Bass)
            return new GuitarBassMapper(session, format, animations.Guitar);

        throw new NotImplementedException();
    }
}
