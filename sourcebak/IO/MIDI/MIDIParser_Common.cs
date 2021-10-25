using System.Collections.Generic;

namespace ChartTools.IO.MIDI
{
    internal static partial class MIDIParser
    {
        private static readonly Dictionary<Instruments, string> sequenceNames = new()
        {
            { Instruments.LeadGuitar, "PART GUITAR" },
            { Instruments.CoopGuitar, "PART GUITAR COOP" },
            { Instruments.Bass, "PART BASS" },
            { Instruments.RhythmGuitar, "PART RHYTHM" },
            { Instruments.Keys, "PART KEYS" },
            { Instruments.Drums, "PART DRUMS" },
            { Instruments.GHLGuitar, "PART GUITAR GHL" },
            { Instruments.GHLBass, "PART BASS GHL" }
        };

        private const string globalEventSequenceName = "EVENTS";
        private const string lyricsSequenceName = "VOCALS";

        private enum NoteMode : byte { Regular, Open, Tap }
    }
}
