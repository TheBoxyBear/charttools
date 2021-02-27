using System.Collections.Generic;

namespace ChartTools.IO.MIDI
{
    internal static partial class MIDIParser
    {
        private static readonly Dictionary<Instruments, string> sequenceNames = new Dictionary<Instruments, string>()
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
    }
}
