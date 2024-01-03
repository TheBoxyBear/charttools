namespace ChartTools.IO.Configuration;

/// <summary>
/// Configuration object to direct the reading or writing of a file
/// </summary>
/// <remarks>If <see langword="null"/>, the default configuration for the file format will be used.</remarks>
public record CommonConfiguration
{
    /// <inheritdoc cref="Configuration.DuplicateTrackObjectPolicy"/>
    public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
    /// <inheritdoc cref="OverlappingSpecialPhrasePolicy"/>
    public OverlappingSpecialPhrasePolicy OverlappingStarPowerPolicy { get; init; }
    /// <inheritdoc cref="Configuration.SnappedNotesPolicy"/>
    public SnappedNotesPolicy SnappedNotesPolicy { get; init; }
    /// <inheritdoc cref="Configuration.SoloNoStarPowerPolicy"/>
    public SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
}
