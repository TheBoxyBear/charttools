namespace ChartTools;

/// <inheritdoc cref="IReadOnlyTrackObject"/>
public interface ITrackObject : IReadOnlyTrackObject
{
	/// <inheritdoc cref="IReadOnlyTrackObject.Position"/>
	public new uint Position { get; set; }

#if NETCOREAPP3_0_OR_GREATER
	uint IReadOnlyTrackObject.Position => Position;
#endif
}
