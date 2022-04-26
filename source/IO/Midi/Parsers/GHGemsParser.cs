using ChartTools;
using ChartTools.Events;
using ChartTools.IO.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsers
{
    internal class GHGemsParser : StandardInstrumentParser
    {
        private readonly uint previousPosition = 0;

        public GHGemsParser(ReadingSession session) : base(StandardInstrumentIdentity.LeadGuitar, session) { }

        protected override void HandleItem(MidiEvent item)
        {
            if (item is not NoteEvent e)
                return;

            (var track, var adjusted) = MapNoteEvent(e);

            if (track is null)
                return;

            if (adjusted < 5) // Note
            {
                var lane = (StandardLane)adjusted;

                switch (e)
                {
                    case NoteOnEvent:
                        break;
                    case NoteOffEvent:
                        break;
                }
            }
            else if (adjusted < 11) // Special
            {
                switch (e)
                {
                    case NoteOnEvent:
                        break;
                    case NoteOffEvent:
                        break;
                }
            }
        }

        protected override (Track<StandardChord>? track, int adjustedNoteNumber) MapNoteEvent(NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            return intNumber switch
            {
                > 59 and < 71 => (result.Easy, intNumber - 60),
                > 71 and < 83 => (result.Medium, intNumber - 72),
                > 83 and < 95 => (result.Hard, intNumber - 84),
                > 95 and < 107 => (result.Expert, intNumber - 96),
                _ => (null, 0)
            };
        }
    }
}
