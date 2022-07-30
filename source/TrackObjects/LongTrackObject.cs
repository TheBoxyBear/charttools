namespace ChartTools
{
    public class LongTrackObject : TrackObject, ILongTrackObject
    {
        public uint Length { get; set; }
        public uint EndPosition => Position + Length;

        public LongTrackObject(uint position) : base(position) { }
    }
}
