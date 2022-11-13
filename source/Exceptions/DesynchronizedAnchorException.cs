namespace ChartTools;

/// <summary>
/// Exception thrown when an invalid operation is performed on a desynchronized anchored <see cref="Tempo"/>.
/// </summary>
public class DesynchronizedAnchorException : Exception
{
    public TimeSpan Anchor { get; }

    public DesynchronizedAnchorException(TimeSpan anchor) : this(anchor, $"Invalid operation performed with desynchronized anchored tempo at {anchor}.") { }
    public DesynchronizedAnchorException(TimeSpan anchor, string message) : base(message) => Anchor = anchor;
}
