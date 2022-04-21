namespace ChartTools
{
    public class LongTrackObject : TrackObject, ILongTrackObject
    {
        public uint Length { get; set; }

        public LongTrackObject(uint position) : base(position) { }
    }
}
