using ChartTools;
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

            (Track<StandardChord> track, int adjusted) = MapNoteEvent(e);

            switch (e)
            {
                case NoteOnEvent:
                    if (adjusted == 6)
                        track.SpecialPhrases.Add(new(previousPosition + (uint)e.DeltaTime, SpecialPhraseType.StarPowerGain));
                    break;
                case NoteOffEvent:
                    break;
            }
        }

        protected override (Track<StandardChord> track, int adjustedNoteNumber) MapNoteEvent(NoteEvent e)
        {
            var intNumber = (int)e.NoteNumber;

            return intNumber switch
            {
                > 59 and < 68 => (result.Easy, intNumber - 60),
                > 71 and < 80 => (result.Medium, intNumber - 72),
                > 83 and < 92 => (result.Hard, intNumber - 84),
                > 95 and < 104 => (result.Expert, intNumber - 96)
            };
        }
    }
}
