using System.Collections;

namespace ChartTools.Animations;
public abstract record AnimationTrack<TEvent> : AnimationTrack, IList<TEvent> where TEvent : AnimationEvent, new()
{
    private readonly List<TEvent> _items;

    public new TEvent this[int index]
    {
        get => _items[index];
        set => _items[index] = value;
    }

    public override int Count => _items.Count;
    public bool IsReadOnly => false;

    public AnimationTrack() => _items = new();
    public AnimationTrack(IEnumerable<TEvent> items) => _items = items.ToList();

    protected override IReadOnlyList<AnimationEvent> GetItems() => _items;

    public void Add(TEvent item) => _items.Add(item);
    public void AddRange(IEnumerable<TEvent> items) => _items.AddRange(items);
    public void Clear() => _items.Clear();
    public bool Contains(TEvent item) => _items.Contains(item);
    public void CopyTo(TEvent[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);
    public int IndexOf(TEvent item) => _items.IndexOf(item);
    public void Insert(int index, TEvent item) => _items.Insert(index, item);
    public bool Remove(TEvent item) => _items.Remove(item);
    public void RemoveAt(int index) => _items.RemoveAt(index);

    public override TEvent Create(uint position) => new() { Position = position };

    public new IEnumerator<TEvent> GetEnumerator() => _items.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
