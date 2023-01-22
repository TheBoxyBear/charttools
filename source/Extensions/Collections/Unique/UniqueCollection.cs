using System.Collections;

namespace ChartTools.Extensions.Collections;

/// <summary>
/// List where all <typeparamref name="T"/> items must be unique through a key
/// </summary>
/// <typeparam name="T">Type of the contained items</typeparam>
public class UniqueCollection<TKey, T> : ICollection<T>
{
    private readonly Dictionary<TKey, T> _items = new();
    private readonly Func<T, TKey> _keySelector;

    /// <summary>
    /// Creates an instance of <see cref="UniqueList{T}"/> using a comparison to define the uniqueness of items.
    /// </summary>
    /// <param name="keySelector">Method that returns the key from an item</param>
    public UniqueCollection(Func<T, TKey> keySelector, IEnumerable<T>? items = null)
    {
        if (keySelector is null)
            throw new ArgumentNullException(nameof(keySelector));

        _keySelector = keySelector;

        if (items is not null)
            AddRange(items);
    }

    /// <inheritdoc/>
    public int Count => _items.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Adds an item to the list and overwrites any duplicate.
    /// </summary>
    /// <param name="item">Item to add</param>
    public virtual void Add(T item)
    {
        RemoveDuplicate(item);
        _items.Add(_keySelector(item), item);
    }

    /// <summary>
    /// Adds multiple items to the <see cref="ICollection{T}"/>
    /// </summary>
    /// <param name="collection">Items to add</param>
    public virtual void AddRange(IEnumerable<T> collection)
    {
        foreach (var item in collection)
            RemoveDuplicate(item);

        foreach (var item in collection)
            _items.Add(_keySelector(item), item);
    }

    /// <summary>
    /// Removes the first duplicate of a given item.
    /// </summary>
    /// <param name="item">Item to remove the duplicate of</param>
    private void RemoveDuplicate(T item)
    {
        var key = _keySelector(item);

        if (_items.ContainsKey(key))
            _items.Remove(key);
    }

    /// <inheritdoc/>
    public void Clear() => _items.Clear();

    /// <inheritdoc/>
    public bool Contains(T item) => _items.ContainsKey(_keySelector(item));

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => _items.Values.CopyTo(array, arrayIndex);

    /// <inheritdoc/>>
    public IEnumerator<T> GetEnumerator() => _items.Values.GetEnumerator();

    /// <inheritdoc/>
    public bool Remove(T item) => _items.Remove(_keySelector(item));

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
