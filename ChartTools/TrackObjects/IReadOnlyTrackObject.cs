namespace ChartTools;

/// <summary>
/// Object located on a track
/// </summary>
public interface IReadOnlyTrackObject
{
    /// <summary>
    /// Tick number on the track.
    /// </summary>
    /// <remarks>A tick represents a subdivision of a beat. The number of subdivisions per beat is stored in <see cref="IO.Formatting.FormattingRules.Resolution"/>.</remarks>
    public uint Position { get; }
}
