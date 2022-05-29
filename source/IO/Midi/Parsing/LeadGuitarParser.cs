using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Midi.Parsing
{
    internal class LeadGuitarParser : StandardInstrumentParser
    {
        public LeadGuitarParser(ReadingSession session) : base(StandardInstrumentIdentity.LeadGuitar, session) { }

        protected override MappingResult<StandardChord> MapNoteEvent(NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            (var track, var adjusted) = intNumber switch
            {
                > 59 and < 71 => (GetOrCreateTrack(Difficulty.Easy), intNumber - 59),
                > 71 and < 83 => (GetOrCreateTrack(Difficulty.Medium), intNumber - 71),
                > 83 and < 95 => (GetOrCreateTrack(Difficulty.Hard), intNumber - 83),
                > 95 and < 107 => (GetOrCreateTrack(Difficulty.Expert), intNumber - 95),
                110 => (null, intNumber),
                _ => (null, 0)
            };
            (var type, var newAdjusted) = adjusted switch
            {
                8 => (MappingType.Special, 1),
                10 => (MappingType.Special, 3),
                11 => (MappingType.Special, 4),
                110 => (MappingType.Modifier, (int)StandardChordModifier.Big),
                _ => (MappingType.Special, adjusted)
            };

            return new MappingResult<StandardChord>(track, type, (byte)newAdjusted);
        }
    }
}
