using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Chart.Configuration.Sessions;

internal class ChartWritingSession(ChartWritingConfiguration? config, FormattingRules? formatting)
	: ChartSession(formatting)
{
#if NET5_0_OR_GREATER
	public override ChartWritingConfiguration Configuration { get; } = config ?? ChartFile.DefaultWriteConfig;
#else
	public new ChartWritingConfiguration Configuration { get; } = config ?? ChartFile.DefaultWriteConfig;

	protected override IO.Configuration.Common.ICommonConfiguration GetCommonConfiguration()
		=> Configuration;
#endif

	public IEnumerable<TrackObjectEntry> GetUnsupportedModifierChordEntries(Chord? previous, Chord current)
		=> Configuration.UnsupportedModifierPolicy switch
	{
		UnsupportedModifierPolicy.ThrowException => throw new Exception($"Chord at position {current.Position} as an unsupported modifier for the chart format."),
		UnsupportedModifierPolicy.IgnoreChord    => [],
		UnsupportedModifierPolicy.IgnoreModifier => current.GetChartNoteData(),
		UnsupportedModifierPolicy.Convert        => current.GetChartModifierData(previous, this),
		_ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnsupportedModifierPolicy)
	};
}
