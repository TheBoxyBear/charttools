using ChartTools.Extensions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart;

public class ChartSection : SectionSet<string>
{
    public static readonly ReservedSectionHeaderSet DefaultReservedHeaders;
    public override ReservedSectionHeaderSet ReservedHeaders => DefaultReservedHeaders;

    static ChartSection()
    {
        var headers = new List<ReservedSectionHeader>
        {
            new(ChartFormatting.MetadataHeader, nameof(Song.Metadata)),
            new(ChartFormatting.SyncTrackHeader, nameof(Song.SyncTrack)),
            new(ChartFormatting.GlobalEventHeader, nameof(Song.GlobalEvents))
        };

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

        headers.AddRange(instrumentSources.SelectMany(pair => from diff in EnumCache<Difficulty>.Values select new ReservedSectionHeader(ChartFormatting.Header(pair.Value, diff), $"{pair.Value}.{diff}")));

        DefaultReservedHeaders = new(headers);
    }
}
