namespace ChartTools;

/// <summary>
/// Exception thrown when an invalid operation is performed on a desynchronized anchored <see cref="Tempo"/>.
/// </summary>
public class DesynchronizedAnchorException(TimeSpan anchor, string message) : Exception(message)
{
    public TimeSpan Anchor { get; } = anchor;

    public DesynchronizedAnchorException(TimeSpan anchor) : this(anchor, $"Invalid operation performed with desynchronized anchored tempo at {anchor}.") { }
}
