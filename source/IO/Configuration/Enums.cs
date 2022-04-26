namespace ChartTools.IO.Configuration
{
    /// <summary>
    /// Defines how overlapping star power phrases should be handled
    /// </summary>
    public enum OverlappingSpecialPhrasePolicy : byte
    {
        ThrowException,
        Ignore,
        /// <summary>
        /// The length of the phrase is cut short to the start of the next one
        /// </summary>
        Cut
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
    /// Defines how duplicate track objects are handled
    /// </summary>
    public enum DuplicateTrackObjectPolicy : byte
    {
        /// <summary>
        /// Throw an exception
        /// </summary>
        ThrowException,
        /// <summary>
        /// Only include the first object
        /// </summary>
        IncludeFirst,
        /// <summary>
        /// Include all objects
        /// </summary>
        IncludeAll,
    }
    /// <summary>
    /// Defines how to handle chord modifiers not supported by the target format
    /// </summary>
    public enum UnsupportedModifierPolicy : byte
    {
        /// <summary>
        /// Throw an exception
        /// </summary>
        ThrowException,
        Convert,
        /// <summary>
        /// The modifier is excluded
        /// </summary>
        IgnoreModifier,
        /// <summary>
        /// The chord is excluded
        /// </summary>
        IgnoreChord,
    }

    public enum UnclosedNotePolicy : byte
    {
        ThrowException,
        Include,
        Ignore
    }
}
