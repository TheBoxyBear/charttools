using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal class GuitarBassMapper : StandardInstrumentMapper, IAnimationContainer<HandPositionEvent>
{
    public override byte BigRockCount => 5;

    public override MidiInstrumentOrigin Format => _format;

    public IEnumerable<HandPositionEvent> AnimationEvents => animations;
    private readonly List<HandPositionEvent> animations = new();

    private MidiInstrumentOrigin _format = MidiInstrumentOrigin.NA;

    public GuitarBassMapper(ReadingSession session) : base(session) { }
    public GuitarBassMapper(WritingSession session, MidiInstrumentOrigin writingFormat, IEnumerable<HandPositionEvent> handAnimations) : base(session)
    {
        if (writingFormat is not MidiInstrumentOrigin.GuitarHero2 or MidiInstrumentOrigin.RockBand)
            throw new NotSupportedException($"Cannot use {nameof(GuitarBassMapper)} to write in format {_format}");

        _format = writingFormat;
        animations = handAnimations.ToList();
    }

    public override IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e)
    {
        if (Format == MidiInstrumentOrigin.NA)
            throw new InvalidOperationException($"No writing format provided");

        var byteNumber = (byte)e.NoteNumber;

        if (byteNumber is 126 or 127)
        {
            var specialType = byteNumber switch
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

        if (byteNumber is > 119 and < 125)
        {
            ApplyFormat(MidiInstrumentOrigin.RockBand);

            yield return CreateMapping(null, MappingType.BigRock, (byte)(125 - byteNumber));
            yield break;
        }

        if (byteNumber is > 39 and < 60)
        {
            ApplyFormat(MidiInstrumentOrigin.GuitarHero2);

            animations.Add(new(position, AnimationMapper.GetHandPositionIndex(byteNumber)));
            yield break;
        }

        if (byteNumber is 116)
        {
            ApplyFormat(MidiInstrumentOrigin.RockBand);

            yield return CreateMapping(null, MappingType.Special, (byte)TrackSpecialPhraseType.StarPowerGain);
            yield break;
        }

        (var difficulty, var adjusted) = byteNumber switch
        {
            > 59 and < 71 => (Difficulty.Easy, byteNumber - 59),
            > 71 and < 83 => (Difficulty.Medium, byteNumber - 71),
            > 83 and < 95 => (Difficulty.Hard, byteNumber - 83),
            > 95 and < 107 => (Difficulty.Expert, byteNumber - 95),
            110 => (default(Difficulty?), byteNumber),
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
                _format = format;
        }
    }

    public override IEnumerable<NoteMapping> Map(Instrument<StandardChord> instrument)
    {
        throw new NotImplementedException();
    }
}
