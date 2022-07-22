using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal class GHGemsMapper : InstrumentMapper<StandardChord>
    {
        public override IEnumerable<TrackObjectMappingResult> MapInstrument(Instrument<StandardChord> instrument)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<MidiMappingResult> MapNoteEvent(uint position, NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            (var difficulty, var adjusted) = intNumber switch
            {
                > 59 and < 71 => (Difficulty.Easy, intNumber - 59),
                > 71 and < 83 => (Difficulty.Medium, intNumber - 71),
                > 83 and < 95 => (Difficulty.Hard, intNumber - 83),
                > 95 and < 107 => (Difficulty.Expert, intNumber - 95),
                _ => (default(Difficulty?), 0)
            };
            (var type, var newAdjusted) = adjusted switch
            {
                8 => (MappingType.Special, (int)SpecialPhraseType.StarPowerGain),
                10 => (MappingType.Special, (int)SpecialPhraseType.Player1FaceOff),
                11 => (MappingType.Special, (int)SpecialPhraseType.Player2FaceOff),
                _ => (MappingType.Note, adjusted)
            };

            yield return new MidiMappingResult(position, GetState(e), difficulty, type, (byte)newAdjusted);
        }
    }
}
