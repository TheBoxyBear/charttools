namespace ChartTools.IO
{
    /// <summary>
    /// Defines how overlapping star power phrases should be handled
    /// </summary>
    public enum OverlappingStarPowerPolicy : byte
    {
        Ignore,
        /// <summary>
        /// The length of the phrase is cut short to the start of the next one
        /// </summary>
        Cut,
        ThrowException
    }
    /// <summary>
    /// Difficulty of the <see cref="Track"/> to serve as a source of for track objects common to all difficulties to use for all tracks in the same <see cref="Instrument"/>
    /// <para>Common track objects are:<list type="bullet">
    /// <item>Local events</item>
    /// <item>Star power phrases</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <remarks>Can be casted from <see cref="Difficulty"/>.</remarks>
    public enum TrackObjectSource : byte
    {
        Easy, Medium, Hard, Expert,
        /// <summary>
        /// Each <see cref="Track"/> will contain a combination of all unique common track objects in the same <see cref="Instrument"/>
        /// </summary>
        Merge,
        Seperate
    }
    /// <summary>
    /// Defines how to handle "solo" local events in tracks
    /// </summary>
    public enum SoloNoStarPowerPolicy : byte
    {
        /// <summary>
        /// Local events are interpreted as is
        /// </summary>
        Ignore,
        /// <summary>
        /// If a track has "solo" or "soloend" local events and no star power, convert the events into star power as interpreted by Clone Hero
        /// </summary>
        Convert
    }
    /// <summary>
    /// Defines how chords with a set of incompatible modifiers are handled
    /// </summary>
    public enum IncompatibleModifierCombinationPolicy : byte
    {
        /// <summary>
        /// All modifiers are included
        /// </summary>
        IncludeAll,
        /// <summary>
        /// Only the first modifier is included
        /// </summary>
        IncludeFirst,
        /// <summary>
        /// The modifiers are excluded
        /// </summary>
        IgnoreModifers,
        /// <summary>
        /// The chord is excluded
        /// </summary>
        IgnoreChord,
        ThrowException
    }
    /// <summary>
    /// Defines how duplicate track objects are handled
    /// </summary>
    public enum DuplicateTrackObjectPolicy : byte
    {
        /// <summary>
        /// Only include the first object
        /// </summary>
        IncludeFirst,
        /// <summary>
        /// Include all objects
        /// </summary>
        IncludeAll,
        ThrowException
    }
    /// <summary>
    /// Hopo threshold to use if included in the metadata and configuration
    /// </summary>
    public enum HopoThresholdPriority : byte
    {
        /// <summary>
        /// Get the threshold from metadata
        /// </summary>
        Metadata,
        /// <summary>
        /// Get the threshold from configuration
        /// </summary>
        Configuration
    }
    /// <summary>
    /// Defines how to handle chord modifiers not supported by the target format
    /// </summary>
    public enum UnsupportedModifierPolicy : byte
    {
        /// <summary>
        /// The modifier is excluded
        /// </summary>
        IgnoreModifier,
        /// <summary>
        /// The chord is excluded
        /// </summary>
        IgnoreChord,
        ThrowException
    }
    public enum UnsupportedVocalsSource : byte
    {
        GlobalEvents,
        Vocals
    }

    public class CommonConfiguration
    {
        /// <inheritdoc cref="IO.OverlappingStarPowerPolicy"/>
        public OverlappingStarPowerPolicy OverlappingStarPowerPolicy { get; init; }
        /// <inheritdoc cref="IO.SoloNoStarPowerPolicy"/>
        public SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
        /// <inheritdoc cref="IO.DuplicateTrackObjectPolicy"/>
        public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
    }

    /// <summary>
    /// Configuration object to direct the reading of a file
    /// </summary>
    public class ReadingConfiguration : CommonConfiguration
    {
        /// <inheritdoc cref="IO.IncompatibleModifierCombinationPolicy"/>
        public IncompatibleModifierCombinationPolicy IncompatibleModifierCombinationPolicy { get; init; }
    }

    public class WritingConfiguration : CommonConfiguration
    {
        public TrackObjectSource EventSource { get; init; }
        public TrackObjectSource StarPowerSource { get; init; }
        /// <inheritdoc cref="IO.HopoThresholdPriority"/>
        public HopoThresholdPriority HopoThresholdPriority { get; init; }
        /// <summary>
        /// *Currently unsupported*
        /// </summary>
        public uint? HopoTreshold { get; init; } = null;
        /// <summary>
        /// *Currently unsupported*
        /// </summary>
        public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
        /// <summary>
        /// *Currently unsupported*
        /// </summary>
        public UnsupportedVocalsSource UnsupportedVocalsSource { get; init; }
    }
}
