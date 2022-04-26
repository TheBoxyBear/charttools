namespace ChartTools.IO.Configuration
{
    public class WritingConfiguration : CommonConfiguration
    {
        /// <summary>
        /// Defines which difficulty to get local events from
        /// </summary>
        public TrackObjectSource EventSource { get; init; }
        public TrackObjectSource StarPowerSource { get; init; }
        public uint? HopoTreshold { get; init; }
        /// <see cref="Configuration.UnsupportedModifierPolicy"/>
        public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
    }
}
