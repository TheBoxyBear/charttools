﻿using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Midi.Mapping
{
    internal class GuitarBassMapper : InstrumentMapper<StandardChord>
    {
        public override IEnumerable<NoteEventMapping> Map(GlobalNoteEvent e, ReadingSession session)
        {
            var intNumber = (int)e.Event.NoteNumber;

            if (intNumber is 126 or 127)
            {
                var specialType = intNumber switch
                {
                    126 => (byte)TrackSpecialPhraseType.Trill,
                    127 => (byte)TrackSpecialPhraseType.Tremolo
                };

                yield return CreateMapping(Difficulty.Expert, MappingType.Special, specialType);

                if ((byte)e.Event.Velocity is > 40 and < 51)
                    yield return CreateMapping(Difficulty.Hard, MappingType.Special, specialType);

                yield break;
            }

            if (intNumber is > 119 and < 125)
            {
                yield return CreateMapping(null, MappingType.BigRock, (byte)(125 - intNumber));
                yield break;
            }

            if (intNumber is < 60)
            {
                yield return CreateMapping(null, MappingType.Animation, 0); // TODO Map animation indexes
                yield break;
            }
            if (intNumber is 116)
            {
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
                _ => HandleInvalidMidiEvent<(Difficulty?, int)>(e, session)
            };
            (var type, var newAdjusted) = adjusted switch
            {
                6 => (MappingType.Modifier, (int)StandardChordModifier.ForcedHopo),
                7 => (MappingType.Modifier, (int)StandardChordModifier.ForcedStrum),
                8 => (MappingType.Special, (int)TrackSpecialPhraseType.StarPowerGain),
                10 => (MappingType.Special, (int)TrackSpecialPhraseType.Player1FaceOff),
                11 => (MappingType.Special, (int)TrackSpecialPhraseType.Player2FaceOff),
                110 => (MappingType.Modifier, (int)StandardChordModifier.Big),
                _ => (MappingType.Note, adjusted)
            };

            yield return CreateMapping(difficulty, type, (byte)newAdjusted);

            NoteEventMapping CreateMapping(Difficulty? difficulty, MappingType type, byte index) => new(e.Position, GetState(e.Event), difficulty, type, index);
        }

        public override IEnumerable<TrackObjectMapping> Map(Instrument<StandardChord> instrument, WritingSession session)
        {
            throw new NotImplementedException();
        }
    }
}
