namespace ChartTools.IO.Configuration
{
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
