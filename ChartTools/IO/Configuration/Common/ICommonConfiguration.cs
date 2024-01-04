namespace ChartTools.IO.Configuration.Common;

/// <summary>
/// Configuration object to direct the reading or writing of a file
/// </summary>
/// <remarks>If <see langword="null"/>, the default configuration for the file format will be used.</remarks>
public interface ICommonConfiguration
{
    /// <inheritdoc cref="Configuration.DuplicateTrackObjectPolicy"/>
    public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; }

    /// <inheritdoc cref="OverlappingSpecialPhrasePolicy"/>
    public OverlappingSpecialPhrasePolicy OverlappingStarPowerPolicy { get; }

    /// <inheritdoc cref="Configuration.SnappedNotesPolicy"/>
    public SnappedNotesPolicy SnappedNotesPolicy { get; }
}
