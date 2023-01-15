namespace ChartTools.Sampling;

public interface ISample
{
    public string Name { get; }

    public abstract IEnumerable<ITrackObject> GetItems();
}
