using System;
using System.Collections;
using System.Collections.Generic;

namespace ChartTools.Collections.Sorted
{
    /// <summary>
    /// *Deprecated* Collection where <typeparamref name="T"/> items are always sorted
    /// </summary>
    /// <typeparam name="T">Type of the contained items</typeparam>
    [Obsolete("Unused by non-internal classes")]
    internal class SelfSorted<T> : ICollection<T> where T : IComparable<T>
    {
        /// <summary>
        /// Source of non-sorted items
        /// </summary>
        protected List<T> items;
        /// <inheritdoc/>
        public int Count => items.Count;
        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Creates an instance of <see cref="SelfSorted{T}"/>.
        /// </summary>
        /// <param name="capacity">Number of items that the <see cref="IndexableSelfSorted{TKey, TValue}"/> can initially store</param>
        public SelfSorted(int capacity = 0) => items = new List<T>(capacity);

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <param name="index">Index of the item</param>
        public T this[int index]
        {
            get
            {
                items.Sort();
                return items[index];
            }
            set
            {
                items.Sort();
                items[index] = value;
            }
        }

        /// <inheritdoc/>
        public virtual void Add(T item) => items.Add(item);
        /// <summary>
        /// Adds multiple items to the <see cref="SelfSorted{T}"/>.
        /// </summary>
        /// <param name="items">Items to add</param>
        public virtual void AddRange(IEnumerable<T> items)
        {
            foreach (T t in items)
                Add(t);
        }
        /// <inheritdoc/>
        public void Clear() => items.Clear();
        /// <inheritdoc/>
        public bool Contains(T item) => items.Contains(item);
        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);
        /// <inheritdoc/>
        public bool Remove(T item) => items.Remove(item);
        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            items.Sort();
            return items.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
