namespace ChartTools
{
    public abstract class TrackObjectBase : ITrackObject
    {
        public uint Position { get; set; }

        public TrackObjectBase() : this(0) { }
        public TrackObjectBase(uint position) => Position = position;
    }
}
