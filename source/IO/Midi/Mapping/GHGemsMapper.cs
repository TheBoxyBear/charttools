using ChartTools.IO.Configuration.Sessions;
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

        public override IEnumerable<MidiMappingResult> MapNoteEvent(uint position, NoteEvent e, ReadingSession session)
        {
            var intNumber = (int)e.NoteNumber;

            (var difficulty, var adjusted) = intNumber switch
            {
                > 59 and < 71 => (Difficulty.Easy, intNumber - 59),
                > 71 and < 83 => (Difficulty.Medium, intNumber - 71),
                > 83 and < 95 => (Difficulty.Hard, intNumber - 83),
                > 95 and < 107 => (Difficulty.Expert, intNumber - 95),
                _ => HandleInvalidMidiEvent<(Difficulty?, int)>(position, e, session)
            };

            (var type, var newAdjusted) = adjusted switch
            {
                8 => (MappingType.Special, (int)TrackSpecialPhraseType.StarPowerGain),
                10 => (MappingType.Special, (int)TrackSpecialPhraseType.Player1FaceOff),
                11 => (MappingType.Special, (int)TrackSpecialPhraseType.Player2FaceOff),
                _ => (MappingType.Note, adjusted)
            };

            yield return new MidiMappingResult(position, GetState(e), difficulty, type, (byte)newAdjusted);
        }
    }
}
