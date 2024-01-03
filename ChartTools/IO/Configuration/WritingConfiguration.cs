namespace ChartTools.IO.Configuration;

public record WritingConfiguration : CommonConfiguration
{
    /// <summary>
    /// Defines which difficulty to get local events from
    /// </summary>
    public TrackObjectSource EventSource { get; init; }
    public TrackObjectSource StarPowerSource { get; init; }
    /// <see cref="Configuration.UnsupportedModifierPolicy"/>
    public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
}
