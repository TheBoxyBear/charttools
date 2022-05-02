using ChartTools.IO.Sections;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart
{
    public class ChartSection : SectionSet<string>
    {
        public static readonly ReservedSectionHeaderSet DefaultReservedHeaders;
        public override ReservedSectionHeaderSet ReservedHeaders => DefaultReservedHeaders;

        static ChartSection()
        {
            var headers = new List<ReservedSectionHeader>();

            headers.Add(new(ChartFormatting.MetadataHeader, nameof(Song.Metadata)));
            headers.Add(new(ChartFormatting.SyncTrackHeader, nameof(Song.SyncTrack)));
            headers.Add(new(ChartFormatting.GlobalEventHeader, nameof(Song.GlobalEvents)));

            var difficulties = Enum.GetValues<Difficulty>().ToArray();
            var instrumentSources = new Dictionary<string, string>()
            {
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.LeadGuitar], nameof(Song.Instruments.LeadGuitar) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.RhythmGuitar], nameof(Song.Instruments.RhythmGuitar) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.CoopGuitar], nameof(Song.Instruments.CoopGuitar) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.Bass], nameof(Song.Instruments.Bass) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.Keys], nameof(Song.Instruments.Keys) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.GHLGuitar], nameof(Song.Instruments.GHLGuitar) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.GHLBass], nameof(Song.Instruments.GHLBass) },
                { ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.Drums], nameof(Song.Instruments.Drums) }
            };

            headers.AddRange(instrumentSources.SelectMany(pair => from diff in difficulties select new ReservedSectionHeader(ChartFormatting.Header(pair.Value, diff), $"{pair.Value}.{diff}")));

            DefaultReservedHeaders = new(headers);
        }
        public ChartSection() : base() { }
    }
}
