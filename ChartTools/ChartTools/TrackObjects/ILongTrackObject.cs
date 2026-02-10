namespace ChartTools;

public interface ILongTrackObject : ITrackObject, ILongObject
{
	/// <summary>
	/// Tick number marking the end of the object
	/// </summary>
#if NETCOREAPP3_0_OR_GREATER
	public uint EndPosition => Position + Length;
#else
	public uint EndPosition { get; }
#endif
}
