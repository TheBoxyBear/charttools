using ChartTools.IO.Configuration.Common;
using ChartTools.IO.Formatting;

namespace ChartTools.IO.Configuration;

internal abstract class Session(FormattingRules? formatting)
{
#if NET5_0_OR_GREATER
	public abstract ICommonConfiguration Configuration { get; }
#else
	public ICommonConfiguration Configuration
		=> GetCommonConfiguration();

	protected abstract ICommonConfiguration GetCommonConfiguration();
#endif

	public FormattingRules Formatting { get; set; } = formatting ?? new();

	public bool HandleDuplicate(uint position, string objectType, Func<bool> checkDuplicate)
		=> Configuration.DuplicateTrackObjectPolicy switch
		{
			DuplicateTrackObjectPolicy.ThrowException => checkDuplicate()
				? throw new Exception($"Duplicate {objectType} on position {position}.") : true,
			DuplicateTrackObjectPolicy.IncludeAll     => true,
			DuplicateTrackObjectPolicy.IncludeFirst   => !checkDuplicate(),
			_ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.DuplicateTrackObjectPolicy),
		};

	public bool HandleSnap(uint origin, uint position)
		=> Configuration.SnappedNotesPolicy switch
		{
			SnappedNotesPolicy.ThrowException => throw new Exception($"Note at position {position} is within snapping distance from chord at position {origin}"),
			SnappedNotesPolicy.Snap           => true,
			SnappedNotesPolicy.Ignore         => false,
			_ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.SnappedNotesPolicy)
		};
}
