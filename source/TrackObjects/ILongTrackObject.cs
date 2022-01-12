namespace ChartTools
{
    public interface ILongTrackObject : ITrackObject, ILongObject
    {
        public uint EndPosition => Position + Length;
    }
}
