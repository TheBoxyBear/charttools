namespace ChartTools.IO.Configuration.Common;

/// <summary>
/// Configuration object to direct the reading or writing of a file
/// </summary>
/// <remarks>If <see langword="null"/>, the default configuration for the file format will be used.</remarks>
public record CommonConfiguration
{
    /// <inheritdoc cref="Configuration.DuplicateTrackObjectPolicy"/>
    public required DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }

    /// <inheritdoc cref="Configuration.OverlappingSpecialPhrasePolicy"/>
    public required OverlappingSpecialPhrasePolicy OverlappingSpecialPhrasePolicy { get; init; }

    /// <inheritdoc cref="Configuration.SnappedNotesPolicy"/>
    public required SnappedNotesPolicy SnappedNotesPolicy { get; init; }

    /// <inheritdoc cref="Configuration.SoloNoStarPowerPolicy"/>
    public required SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
}
