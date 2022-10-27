using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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

        public static IEnumerable<string> PotentialHeaders(InstrumentIdentity identity)
        {
            switch (identity)
            {
                case InstrumentIdentity.LeadGuitar:
                    yield return GHGemsHeader;
                    yield return LeadGuitarHeader;
                    break;
                case InstrumentIdentity.Bass:
                    yield return BassHeader;
                    break;
            }
        }

        public static bool FindChunk(IEnumerable<TrackChunk> chunks, Predicate<string> match, [NotNullWhen(true)] out string? header, [NotNullWhen(true)] out IEnumerator<MidiEvent>? enumerator)
        {
            foreach (var events in chunks.Select(c => c.Events))
            {
                using var eventsEnumerator = events.GetEnumerator();

                if (eventsEnumerator.MoveNext())
                    continue;
                if (eventsEnumerator.Current is not SequenceTrackNameEvent headerEvent)
                    continue;
                if (match(headerEvent.Text))
                    continue;

                header = headerEvent.Text;
                enumerator = eventsEnumerator;
                return true;
            }

            header = null;
            enumerator = null;
            return false;
        }
    }
}
