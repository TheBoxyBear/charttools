using ChartTools.Extensions.Collections;

namespace ChartTools.Sampling;

public class SampleSet<T> where T : ISample
{
    public UniqueList<T> Samples { get; } = new((a, b) => a.Name == b.Name);

    public T? Get(string name) => Samples.FirstOrDefault(s => s.Name == name);
}
