using System.Collections.Generic;

namespace ChartTools.IO.Midi
{
    internal static class MidiFormatting
    {
        private static readonly Dictionary<InstrumentIdentity, string> InstrumentSequenceNames = new()
        {
            { InstrumentIdentity.LeadGuitar, LeadGuitarHeader },
            { InstrumentIdentity.CoopGuitar, CoopGuitarHeader },
            { InstrumentIdentity.Bass, BassHeader },
            { InstrumentIdentity.RhythmGuitar, RhythmGuitarHeader },
            { InstrumentIdentity.Keys, KeysHeader },
            { InstrumentIdentity.Drums, DrumsHeader },
            { InstrumentIdentity.GHLGuitar, GHLGuitarHeader },
            { InstrumentIdentity.GHLBass, GHLBassHeader },
            { InstrumentIdentity.Vocals, VocalsHeader }
        };

        public const string
            BassHeader = "PART BASS",
            CoopGuitarHeader = "PART GUITAR COOP",
            DrumsHeader = "PART DRUMS",
            GHGemsHeader = "T1 GEMS",
            GlobalEventHeader = "EVENTS",
            GHLGuitarHeader = "PART GUITAR GHL",
            GHLBassHeader = "PART BASS GHL",
            KeysHeader = "PART KEYS",
            LeadGuitarHeader = "PART GUITAR",
            RhythmGuitarHeader = "PART RHYTHM",
            VocalsHeader = "PART VOCALS";



        public static string Instrument(InstrumentIdentity instrument) => InstrumentSequenceNames[instrument];
    }
}
