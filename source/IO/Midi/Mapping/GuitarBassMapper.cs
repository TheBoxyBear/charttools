using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal class GuitarBassMapper : InstrumentMapper<StandardChord>
{
    public MidiInstrumentOrigin Format { get; private set; }
    public override string Header { get; }

    public GuitarBassMapper(StandardInstrumentIdentity instrument) : this(instrument, MidiInstrumentOrigin.NA) { }
    public GuitarBassMapper(StandardInstrumentIdentity instrument, MidiInstrumentOrigin writingFormat)
    {
        Format = writingFormat;
        Header = instrument == StandardInstrumentIdentity.LeadGuitar ? MidiFormatting.LeadGuitarHeader : MidiFormatting.BassHeader;
    }

    public override IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e)
    {
        if (Format == MidiInstrumentOrigin.NA)
            throw new InvalidOperationException($"No writing format provided");

        var intNumber = (int)e.NoteNumber;

        if (intNumber is 126 or 127)
        {
            var specialType = intNumber switch
            {
                126 => (byte)TrackSpecialPhraseType.Trill,
                127 => (byte)TrackSpecialPhraseType.Tremolo
            };

            yield return CreateMapping(Difficulty.Expert, MappingType.Special, specialType);

            if ((byte)e.Velocity is > 40 and < 51)
                yield return CreateMapping(Difficulty.Hard, MappingType.Special, specialType);

            ApplyFormat(MidiInstrumentOrigin.RockBand);
            yield break;
        }

        if (intNumber is > 119 and < 125)
        {
            ApplyFormat(MidiInstrumentOrigin.RockBand);

            yield return CreateMapping(null, MappingType.BigRock, (byte)(125 - intNumber));
            yield break;
        }

        if (intNumber is > 39 and < 60)
        {
            ApplyFormat(MidiInstrumentOrigin.GuitarHero2);

            yield return CreateMapping(null, MappingType.Animation, (byte)(intNumber - 39));
            yield break;
        }
        if (intNumber is 116)
        {
            ApplyFormat(MidiInstrumentOrigin.RockBand);

            yield return CreateMapping(null, MappingType.Special, (byte)TrackSpecialPhraseType.StarPowerGain);
            yield break;
        }

        (var difficulty, var adjusted) = intNumber switch
        {
            > 59 and < 71 => (Difficulty.Easy, intNumber - 59),
            > 71 and < 83 => (Difficulty.Medium, intNumber - 71),
            > 83 and < 95 => (Difficulty.Hard, intNumber - 83),
            > 95 and < 107 => (Difficulty.Expert, intNumber - 95),
            110 => (default(Difficulty?), intNumber),
            _ => HandleInvalidMidiEvent<(Difficulty?, int)>(position, e)
        };
        (var type, var newAdjusted) = adjusted switch
        {
            6 => (MappingType.Modifier, (int)StandardChordModifiers.ForcedHopo),
            7 => (MappingType.Modifier, (int)StandardChordModifiers.ForcedStrum),
            8 => ApplyStarPower(),
            10 => (MappingType.Special, (int)TrackSpecialPhraseType.Player1FaceOff),
            11 => (MappingType.Special, (int)TrackSpecialPhraseType.Player2FaceOff),
            110 => (MappingType.Modifier, (int)StandardChordModifiers.Big),
            _ => (MappingType.Note, adjusted)
        };


        if (newAdjusted > 5)
            HandleInvalidMidiEvent<StandardChord>(position, e);

        yield return CreateMapping(difficulty, type, (byte)newAdjusted);

        (MappingType, int) ApplyStarPower()
        {
            if (difficulty < Difficulty.Expert)
                ApplyFormat(MidiInstrumentOrigin.GuitarHero2);

            return (MappingType.Special, (int)TrackSpecialPhraseType.StarPowerGain);
        }
        NoteEventMapping CreateMapping(Difficulty? diff, MappingType type, byte index) => new(position, e, diff, type, index);

        void ApplyFormat(MidiInstrumentOrigin format)
        {
            if (format is MidiInstrumentOrigin.Unknown)
                Format = format;
        }
    }

    public override IEnumerable<NoteMapping> Map(Instrument<StandardChord> instrument)
    {
        throw new NotImplementedException();
    }
}
