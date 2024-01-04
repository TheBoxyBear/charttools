namespace ChartTools.IO.Configuration;

/// <summary>
/// Set of policies defining how errors during write operations are handled
/// </summary>
/// <inheritdoc cref="CommonConfiguration"/>
public record WritingConfiguration : CommonConfiguration
{
    /// <summary>
    /// Defines which difficulty to get local events from
    /// </summary>
    public TrackObjectSource EventSource { get; init; }
    public TrackObjectSource StarPowerSource { get; init; }
    /// <see cref="UnsupportedModifiersPolicy"/>
    public UnsupportedModifiersPolicy UnsupportedModifierPolicy { get; init; }
}
