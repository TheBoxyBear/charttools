namespace ChartTools
{
    public class LongTrackObject : TrackObject, ILongObject
    {
        public uint Length { get; set; }

        public LongTrackObject(uint position) : base(position) { }
    }
}
