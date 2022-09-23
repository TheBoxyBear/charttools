namespace ChartTools
{
    public interface ILongTrackObject : ITrackObject, IReadOnlyLongObject
    {
        /// <summary>
        /// Tick number marking the end of the object
        /// </summary>
        public uint EndPosition => Position + Length;
        public uint Length { get; set; }
    }
}
