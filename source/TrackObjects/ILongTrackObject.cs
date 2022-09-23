namespace ChartTools
{
    public interface ILongTrackObject : ITrackObject, IReadOnlyLongObject
    {
        /// <summary>
        /// Tick number marking the end of the object
        /// </summary>
        public uint EndPosition { get; }
        public uint Length { get; set; }
    }
}
