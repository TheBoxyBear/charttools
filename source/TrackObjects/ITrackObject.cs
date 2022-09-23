namespace ChartTools
{
    /// <inheritdoc cref="IReadOnlyTrackObject"/>
    public interface ITrackObject : IReadOnlyTrackObject
    {
        /// <inheritdoc cref="IReadOnlyTrackObject.Position"/>
        public new uint Position { get; set; }
    }
}
