using ChartTools.Extensions;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Midi.Parsing
{
    internal class GHGemsParser : StandardInstrumentParser
    {
        public GHGemsParser(ReadingSession session) : base(StandardInstrumentIdentity.LeadGuitar, session) { }

        protected override IEnumerable<NoteEventMapping> MapNoteEvent(uint position, NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            (var difficulty, var adjusted) = intNumber switch
            {
                > 59 and < 71 => (Difficulty.Easy, intNumber - 59),
                > 71 and < 83 => (Difficulty.Medium, intNumber - 71),
                > 83 and < 95 => (Difficulty.Hard, intNumber - 83),
                > 95 and < 107 => (Difficulty.Expert, intNumber - 95),
                _ => TypePassthrough.Passthrough<(Difficulty?, int)>(() => session.InvalidMidiEventTypeProcedure(position, e))
            };

            (var type, var newAdjusted) = adjusted switch
            {
                8 => (MappingType.Special, (int)TrackSpecialPhraseType.StarPowerGain),
                10 => (MappingType.Special, (int)TrackSpecialPhraseType.Player1FaceOff),
                11 => (MappingType.Special, (int)TrackSpecialPhraseType.Player2FaceOff),
                _ => (MappingType.Note, adjusted)
            };

            if (newAdjusted > 5)
                session.InvalidMidiEventTypeProcedure(position, e);

            yield return new(position, e, difficulty, type, (byte)newAdjusted);
        }
    }
}
