using System.Collections.Generic;

namespace ChartTools.IO.Midi
{
    internal static class MidiFormatting
    {
        private static readonly Dictionary<InstrumentIdentity, string> InstrumentSequenceNames = new()
        {
            { InstrumentIdentity.LeadGuitar, "PART GUITAR" },
            { InstrumentIdentity.CoopGuitar, "PART GUITAR COOP" },
            { InstrumentIdentity.Bass, "PART BASS" },
            { InstrumentIdentity.RhythmGuitar, "PART RHYTHM" },
            { InstrumentIdentity.Keys, "PART KEYS" },
            { InstrumentIdentity.Drums, "PART DRUMS" },
            { InstrumentIdentity.GHLGuitar, "PART GUITAR GHL" },
            { InstrumentIdentity.GHLBass, "PART BASS GHL" },
            { InstrumentIdentity.Vocals, "PART VOCALS" }
        };

        public const string GHGemsHeader = "T1 GEMS";

        public static string Instrument(InstrumentIdentity instrument) => InstrumentSequenceNames[instrument];
    }
}
