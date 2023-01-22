using ChartTools.Tools;

using System.Collections;

namespace ChartTools.Extensions.Collections;

/// <summary>
/// Set of track objects where each one must have a different position
/// </summary>
public class UniqueTrackObjectCollection<T> : ICollection<T> where T : ITrackObject
{
    private readonly Dictionary<uint, T> items;

    public UniqueTrackObjectCollection(IEnumerable<T>? items = null) => this.items = items is null ? new() : items.ToDictionary(i => i.Position);

    public int Count => items.Count;
    bool ICollection<T>.IsReadOnly => false;

    private void RemoveDuplicate(T item)
    {
        if (items.ContainsKey(item.Position))
            items.Remove(item.Position);
    }

    public void Add(T item)
    {
        RemoveDuplicate(item);
        items.Add(item.Position, item);
    }

    public void Clear() => items.Clear();

    public bool Contains(T item) => items.ContainsKey(item.Position);

    public void CopyTo(T[] array, int arrayIndex) => items.Values.CopyTo(array, arrayIndex);

    public bool Remove(T item) => items.Remove(item.Position);

    public IEnumerator<T> GetEnumerator() => items.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
