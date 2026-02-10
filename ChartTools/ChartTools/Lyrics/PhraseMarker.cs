namespace ChartTools.Lyrics;

/// <summary>
/// Marker defining the position and length of a vocals phrase.
/// </summary>
/// <param name="position"></param>
public class PhraseMarker(uint position) : TrackObjectBase(position), ILongTrackObject
{
	/// <summary>
	/// Manual length of the phrase defining a phrase end marker where applicable.
	/// </summary>
	/// <remarks>A value of 0 defines the length to be up to the next phrase start.</remarks>
	public uint Length { get; set; }

#if !NETCOREAPP3_0_OR_GREATER
	uint ILongTrackObject.EndPosition => Position + Length;
#endif
}
