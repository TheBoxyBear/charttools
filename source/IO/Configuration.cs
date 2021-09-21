namespace ChartTools.IO
{

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
    public enum IncompatibleModifiersPolicy
    {
        /// <summary>
        /// All modifiers are included
        /// </summary>
        IncludeAll,
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
    /// Hopo threshold to prioritize if included in the metadata and configuration
    /// </summary>
    public enum HopoThresholdPriority
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
    public enum UnsupportedModifierPolicy
    {
        /// <summary>
        /// The modifier is excluded
        /// </summary>
        IgnoreModifier,
        /// <summary>
        /// The chord is excluded
        /// </summary>
        IgnoreChord,
        /// <summary>
        /// The modifier is converted to a compatible one
        /// </summary>
        Convert,
        ThrowException
    }

    /// <summary>
    /// Configuration object to direct the reading of a file
    /// </summary>
    public class ReadingConfiguration
    {
        /// <inheritdoc cref="SoloNoStarPowerPolicy"/>
        public SoloNoStarPowerPolicy SoloNoStarPowerRule { get; init; }
        /// <summary>
        /// *Unsupported*
        /// </summary>
        public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
        public IncompatibleModifiersPolicy IncompatibleModifiersPolicy { get; init; }
    }

    public class WritingConfiguration
    {
        /// <inheritdoc cref="SoloNoStarPowerPolicy">
        public SoloNoStarPowerPolicy SoloNoStarPowerPolicy { get; init; }
        /// <inheritdoc cref="TrackObjectSource">
        public TrackObjectSource EventSource { get; init; }
        /// <inheritdoc cref="TrackObjectSource"/>
        public TrackObjectSource StarPowerSource { get; init; }
        /// <summary>
        /// *Unsupported*
        /// </summary>
        public DuplicateTrackObjectPolicy DuplicateTrackObjectPolicy { get; init; }
        /// <summary>
        /// *Unsupported*
        /// </summary>
        public HopoThresholdPriority HopoThresholdPriority { get; init; }
        /// <summary>
        /// *Unsupported*
        /// </summary>
        public uint? HopoTreshold { get; init; } = null;
        /// <summary>
        /// *Unsupported*
        /// </summary>
        public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
    }
}
