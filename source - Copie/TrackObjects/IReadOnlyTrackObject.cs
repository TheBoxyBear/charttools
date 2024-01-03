namespace ChartTools;

/// <summary>
/// Object located on a track
/// </summary>
public interface IReadOnlyTrackObject : IEquatable<IReadOnlyTrackObject>
{
    /// <summary>
    /// Tick number on the track.
    /// </summary>
    /// <remarks>A tick represents a subdivision of a beat. The number of subdivisions per beat is stored in <see cref="IO.Formatting.FormattingRules.Resolution"/>.</remarks>
    public uint Position { get; }

    bool IEquatable<IReadOnlyTrackObject>.Equals(IReadOnlyTrackObject? other) => other is not null && other.Position == Position;
}
