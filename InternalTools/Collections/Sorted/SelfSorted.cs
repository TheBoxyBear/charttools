using System;
using System.Collections;
using System.Collections.Generic;

namespace ChartTools.Collections.Sorted
{
    /// <summary>
    /// *Deprecated* Collection where <typeparamref name="T"/> items are always sorted
    /// </summary>
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
        public SelfSorted(int capacity = 0) => items = new List<T>(capacity);

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
