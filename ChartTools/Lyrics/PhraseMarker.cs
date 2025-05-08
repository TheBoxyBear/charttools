namespace ChartTools.Lyrics;

public class PhraseMarker(uint position) : TrackObjectBase(position), ILongTrackObject
{
	/// <summary>
	/// Manual length of the phrase defining a phrase end marker where applicable.
	/// </summary>
	/// <remarks>A value of 0 defines the length to be up to the next phrase.</remarks>
	public uint Length { get; set; }
}
