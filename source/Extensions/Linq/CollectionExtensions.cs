namespace ChartTools.Extensions.Linq;

public static class CollectionExtensions
{
    public static int BinarySearchIndex<T, TKey>(this IList<T> source, TKey target, Func<T, TKey> keySelector, out bool exactMatch) where TKey : notnull, IComparable<TKey>
    {
        int left = 0, right = source.Count - 1, middle, index = 0;

        while (left <= right)
        {
            middle = (left + right) / 2;

            switch (keySelector(source[middle]).CompareTo(target))
            {
                case -1:
                    index = left = middle + 1;
                    break;
                case 0:
                    exactMatch = true;
                    return middle;
                case 1:
                    index = right = middle - 1;
                    break;
            }
        }

        exactMatch = false;
        return index;
    }
    public static int BinarySearchIndex<T>(this IList<T> source, T target, out bool exactMatch) where T : notnull, IComparable<T> => BinarySearchIndex(source, target, t => t, out exactMatch);

    /// <summary>
    /// Removes all items in a <see cref="ICollection{T}"/> that meet a condition
    /// </summary>
    /// <param name="source">Collection to remove items from</param>
    /// <param name="predicate">Function that determines which items to remove</param>
    public static void RemoveWhere<T>(this ICollection<T> source, Predicate<T> predicate)
    {
        foreach (T item in source.Where(i => predicate(i)))
            source.Remove(item);
    }
}
