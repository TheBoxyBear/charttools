namespace ChartTools;

public abstract class TrackObjectBase(uint position) : ITrackObject
{
    public virtual uint Position { get; set; } = position;

    public TrackObjectBase() : this(0) { }
}
