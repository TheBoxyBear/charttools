using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.Sampling;

public class Sample<T> : ISample where T : ITrackObject
{
    public string Name { get; }
    public UniqueTrackObjectCollection<T> Items { get; } = new();

    public Sample(string name) => Name = name;

    IEnumerable<ITrackObject> ISample.GetItems() => Items.Cast<ITrackObject>();
}
