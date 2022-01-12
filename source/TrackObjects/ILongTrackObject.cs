namespace ChartTools
{
    public interface ILongTrackObject : ITrackObject, ILongObject
    {
        /// <summary>
        /// Tick number marking the end of the object
        /// </summary>
        public uint EndPosition => Position + Length;
    }
}
