namespace ChartTools;

public interface ILongObject : IReadOnlyLongObject
{
    /// <inheritdoc cref="IReadOnlyTrackObject.Position"/>
    public new uint Length { get; set; }

    uint IReadOnlyLongObject.Length => Length;
}
