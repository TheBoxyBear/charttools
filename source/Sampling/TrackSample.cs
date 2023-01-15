using ChartTools.Events;

using System.Linq;

namespace ChartTools.Sampling;

public class TrackSample<T> : ISample where T : IChord
{
    public string Name { get; set; }
    public List<LocalEvent> LocalEvents { get; set; }
    public UniqueTrackObjectCollection<T> Chords { get; } = new();

    public TrackSample(string name) => Name = name;

    public IEnumerable<ITrackObject> GetItems() => LocalEvents.Concat(Chords.Cast<ITrackObject>());
}
