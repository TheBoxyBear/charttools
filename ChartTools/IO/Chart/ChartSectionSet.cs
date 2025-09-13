using ChartTools.Extensions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart;

public class ChartSection : SectionSet<string>
{
	public static readonly ReservedSectionHeaderSet DefaultReservedHeaders;
	public override ReservedSectionHeaderSet ReservedHeaders => DefaultReservedHeaders;

	static ChartSection()
	{
		List<ReservedSectionHeader> headers =
		[
			new(ChartFormatting.MetadataHeader,    nameof(Song.Metadata)),
			new(ChartFormatting.SyncTrackHeader,   nameof(Song.SyncTrack)),
			new(ChartFormatting.GlobalEventHeader, nameof(Song.GlobalEvents))
		];

		Dictionary<string, string> instrumentSources = new()
		{
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.StandardLeadGuitar],
				nameof(Song.Instruments.StandardLeadGuitar)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.StandardRhythmGuitar],
				nameof(Song.Instruments.StandardRhythmGuitar)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.StandardCoopGuitar],
				nameof(Song.Instruments.StandardCoopGuitar)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.StandardBass],
				nameof(Song.Instruments.StandardBass)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.StandardKeys],
				nameof(Song.Instruments.StandardKeys)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.GHLLeadGuitar],
				nameof(Song.Instruments.GHLLeadGuitar)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.GHLBass],
				nameof(Song.Instruments.GHLBass)
			},
			{
				ChartFormatting.InstrumentHeaderNames[InstrumentIdentity.Drums],
				nameof(Song.Instruments.Drums)
			}
		};

		headers.AddRange(instrumentSources.SelectMany(pair =>
		from diff in EnumCache<Difficulty>.Values
		select new ReservedSectionHeader(ChartFormatting.Header(pair.Value, diff), $"{pair.Value}.{diff}")));

		DefaultReservedHeaders = new(headers);
	}
}
