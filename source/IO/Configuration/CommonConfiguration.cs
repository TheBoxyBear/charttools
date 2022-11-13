namespace ChartTools.IO.Configuration;

/// <summary>
/// Set of policies defining how errors in IO operations are handled
/// </summary>
/// <remarks>If <see langword="null"/>, the default configuration for the target file type will be used.</remarks>
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
    /// <inheritdoc cref="Configuration.UncertainGuitarBassFormatPolicy"/>
    public UncertainGuitarBassFormatPolicy UncertainGuitarBassFormatPolicy { get; set; }
}
