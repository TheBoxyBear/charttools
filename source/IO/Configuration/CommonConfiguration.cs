namespace ChartTools.IO.Configuration
{
    /// <summary>
    /// Configuration object to direct the reading or writing of a file
    /// </summary>
    /// <remarks>If <see langword="null"/>, the default configuration for the file format will be used.</remarks>
    public class CommonConfiguration
    {
        /// <inheritdoc cref="IO.OverlappingStarPowerPolicy"/>
        public OverlappingSpecialPhrasePolicy OverlappingStarPowerPolicy { get; init; }
        /// <inheritdoc cref="IO.SoloNoStarPowerPolicy"/>
        public SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
        /// <inheritdoc cref="IO.DuplicateTrackObjectPolicy"/>
        public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
    }
}
