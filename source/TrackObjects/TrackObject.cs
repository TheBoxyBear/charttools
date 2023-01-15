namespace ChartTools;

public abstract class TrackObject : ITrackObject
{
    public virtual uint Position { get; set; }

    public TrackObject() : this(0) { }
    public TrackObject(uint position) => Position = position;
}
