namespace ChartTools
{
    public interface ILongObject : IReadOnlyLongObject
    {
        /// <inheritdoc cref="IReadOnlyTrackObject.Position"/>
        public uint Length { get; set; }
    }
}
