using System.Collections.Generic;

namespace ChartTools.IO.MIDI
{
    internal static partial class MIDIParser
    {
        private static readonly Dictionary<InstrumentIdentity, string> sequenceNames = new()
        {
            { InstrumentIdentity.LeadGuitar, "PART GUITAR" },
            { InstrumentIdentity.CoopGuitar, "PART GUITAR COOP" },
            { InstrumentIdentity.Bass, "PART BASS" },
            { InstrumentIdentity.RhythmGuitar, "PART RHYTHM" },
            { InstrumentIdentity.Keys, "PART KEYS" },
            { InstrumentIdentity.Drums, "PART DRUMS" },
            { InstrumentIdentity.GHLGuitar, "PART GUITAR GHL" },
            { InstrumentIdentity.GHLBass, "PART BASS GHL" }
        };

        private const string globalEventSequenceName = "EVENTS";
        private const string lyricsSequenceName = "VOCALS";

        private enum NoteMode : byte { Regular, Open, Tap }
    }
}
