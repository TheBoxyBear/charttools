using ChartTools.Animations;
using ChartTools.IO.Configuration;
using ChartTools.IO.Midi.Configuration.Sessions;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal class GHGemsMapper : StandardInstrumentMapper, IAnimationContainer<VocalistMouthEvent>
{
    public override MidiInstrumentOrigin Format => MidiInstrumentOrigin.GuitarHero1;

    public IEnumerable<VocalistMouthEvent> AnimationEvents => animations;
    private readonly List<VocalistMouthEvent> animations = [];

    public GHGemsMapper(MidiReadingSession session) : base(session) { }
    public GHGemsMapper(MidiWritingSession session, IEnumerable<VocalistMouthEvent> vocalistAnimations) : base(session) => animations = vocalistAnimations.ToList();

    public override IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e)
    {
        if (e.NoteNumber == 108)
        {
            animations.Add(new(position, (VocalistMouthState)NoteEventMapping.GetState(e)));
            yield break;
        }

        var byteNumber = (byte)e.NoteNumber;

        (var difficulty, var adjusted) = byteNumber switch
        {
            > 59 and < 71 => (Difficulty.Easy, byteNumber - 59),
            > 71 and < 83 => (Difficulty.Medium, byteNumber - 71),
            > 83 and < 95 => (Difficulty.Hard, byteNumber - 83),
            > 95 and < 107 => (Difficulty.Expert, byteNumber - 95),
            _ => HandleInvalidMidiEvent<(Difficulty?, int)>(position, e)
        };

        (var type, var newAdjusted) = adjusted switch
        {
            8 => (MappingType.Special, (int)TrackSpecialPhraseType.StarPowerGain),
            10 => (MappingType.Special, (int)TrackSpecialPhraseType.Player1FaceOff),
            11 => (MappingType.Special, (int)TrackSpecialPhraseType.Player2FaceOff),
            _ => (MappingType.Note, adjusted)
        };

        if (newAdjusted > 5)
            HandleInvalidMidiEvent(position, e);

        yield return new(position, e, difficulty, type, (byte)newAdjusted);
    }

    public override IEnumerable<NoteMapping> Map(Instrument<StandardChord> instrument)
    {
        foreach (var track in instrument.GetExistingTracks())
        {
            var offset = track.Difficulty switch
            {
                Difficulty.Easy => 60,
                Difficulty.Medium => 72,
                Difficulty.Hard => 84,
                Difficulty.Expert => 96,
                _ => throw new UndefinedEnumException(track.Difficulty)
            };

            foreach (var chord in track.Chords.Where(c => c.Modifiers == StandardChordModifiers.None || WritingSession!.UnsupportedModifiersProcedure(c).HasFlag(UnsupportedModifiersResults.Chord)))
                foreach (var note in chord.Notes)
                {
                    var sevenBitIndex = new SevenBitNumber((byte)(note.Index + offset));

                    yield return new(chord.Position, NoteState.Open, sevenBitIndex);
                    yield return new(chord.Position + note.Sustain, NoteState.Close, sevenBitIndex);
                }

            foreach (var special in track.SpecialPhrases)
            {
                var index = special.Type switch
                {
                    TrackSpecialPhraseType.StarPowerGain => 8,
                    TrackSpecialPhraseType.Player1FaceOff => 10,
                    TrackSpecialPhraseType.Player2FaceOff => 11,
                    _ => -1
                };

                if (index is not -1)
                {
                    var sevenBitIndex = new SevenBitNumber((byte)(index + offset));

                    yield return new(special.Position, NoteState.Open, sevenBitIndex);
                    yield return new(special.EndPosition, NoteState.Close, sevenBitIndex);
                }
            }

            foreach (var animation in animations)
                yield return new(animation.Position, (NoteState)animation.State, (SevenBitNumber)108);
        }
    }
}
