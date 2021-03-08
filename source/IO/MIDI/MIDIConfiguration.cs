namespace ChartTools.IO.MIDI
{
    /// <summary>
    /// Difficulty of the <see cref="Track"/> to serve as a source of local events to use for all tracks in the same <see cref="Instrument"/>
    /// </summary>
    /// <remarks>Can be casted from <see cref="Difficulty"/>.</remarks>
    public enum LocalEventSource : byte
    {
        Easy, Medium, Hard, Expert,
        /// <summary>
        /// Each <see cref="Track"/> will contain a combinaition of all unique local events in the same <see cref="Instrument"/>
        /// </summary>
        Merge,
    }
    public enum StarPowerSource
    {
        Easy, Medium, Hard, Expert,
        Merge
    }

    /// <summary>
    /// Defines how to handle "solo" local events in tracks 
    /// </summary>
    public enum SoloNoStarPowerRule : byte
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

    public class MIDIReadingConfiguration
    {
        /// <inheritdoc cref="MIDI.SoloNoStarPowerRule">
        public SoloNoStarPowerRule SoloNoStarPowerRule { get; set; } = SoloNoStarPowerRule.Convert;
    }

    public class MIDIWritingConfiguration
    {
        /// <inheritdoc cref="MIDI.SoloNoStarPowerRule">
        public SoloNoStarPowerRule SoloNoStarPowerRule { get; set; } = SoloNoStarPowerRule.Convert;
        /// <inheritdoc cref="LocalEventSource">
        public LocalEventSource EventSource { get; set; } = LocalEventSource.Merge;
        public StarPowerSource StarPowerSource { get; set; } = StarPowerSource.Merge;
    }
}
