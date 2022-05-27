namespace ChartTools
{
    public interface ITrackObject : IReadOnlyTrackObject
    {
        /// <inheritdoc cref="IReadOnlyTrackObject.Position"/>
        public new uint Position { set; }
    }
}
