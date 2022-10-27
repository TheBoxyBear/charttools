﻿using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Mapping
{
    internal class GHGemsMapper : InstrumentMapper<StandardChord>
    {
        public override string Header => MidiFormatting.GHGemsHeader;

        public override IEnumerable<NoteEventMapping> Map(uint position, NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            (var difficulty, var adjusted) = intNumber switch
            {
                > 59 and < 71 => (Difficulty.Easy, intNumber - 59),
                > 71 and < 83 => (Difficulty.Medium, intNumber - 71),
                > 83 and < 95 => (Difficulty.Hard, intNumber - 83),
                > 95 and < 107 => (Difficulty.Expert, intNumber - 95),
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

        public override IEnumerable<NoteMapping> Map(Instrument<StandardChord> instrument) => instrument.GetExistingTracks().SelectMany(Map);
        public override IEnumerable<NoteMapping> Map(Track<StandardChord> track)
        {
            var offset = track.Difficulty switch
            {
                Difficulty.Easy => 60,
                Difficulty.Medium => 72,
                Difficulty.Hard => 84,
                Difficulty.Expert => 96,
                _ => throw new UndefinedEnumException(track.Difficulty)
            };

            foreach (var chord in track.Chords.Where(c => c.Modifiers == StandardChordModifiers.None || WritingSession!.UnsupportedModifiersProcedure(c).HasFlag(Configuration.UnsupportedModifiersResults.Chord)))
                foreach (var note in chord.Notes)
                {
                    var sevenBitIndex = new SevenBitNumber((byte)(note.Index + offset));

                    yield return new(chord.Position, track.Difficulty, NoteState.Open, sevenBitIndex);
                    yield return new(chord.Position + note.Sustain, track.Difficulty, NoteState.Close, sevenBitIndex);
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

                    yield return new(special.Position, track.Difficulty, NoteState.Open, sevenBitIndex);
                    yield return new(special.EndPosition, track.Difficulty, NoteState.Close, sevenBitIndex);
                }
            }
        }
    }
}
