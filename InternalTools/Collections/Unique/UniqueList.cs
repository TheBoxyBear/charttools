using ChartTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections
{
    /// <summary>
    /// List where all <typeparamref name="T"/> items must be unique using a given comparison
    /// </summary>
    public class UniqueList<T> : IList<T>
    {
        /// <summary>
        /// Source of items
        /// </summary>
        protected List<T> items;
        /// <inheritdoc cref="Comparison{T}"/>
        protected Comparison<T> Comparison { get; }

        /// <summary>
        /// Creates an instance of <see cref="UniqueList{T}"/> using a comparison to define the uniqueness of items.
        /// </summary>
        public UniqueList(Comparison<T> comparison, int capacity = 0, IEnumerable<T> items = null)
        {
            Comparison = comparison;
            this.items = new List<T>(capacity);

            if (items is not null)
                foreach (T item in items)
                    this.items.Add(item);
        }
        /// <inheritdoc/>
        public T this[int index] { get => items[index]; set => items[index] = value; }

        public int Count => items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the list and overwrites any duplicate.
        /// </summary>
        public void Add(T item)
        {
            RemoveDuplicate(item);
            items.Add(item);
        }
        /// <summary>
        /// Adds an item wihout checking for uniqueness.
        /// </summary>
        internal void UnsafeAdd(T item) => items.Add(item);
        
        /// <summary>
        /// Adds multiple items to the <see cref="ICollection{T}"/>
        /// </summary>
        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                RemoveDuplicate(item);

            items.AddRange(collection);
        }

        /// <summary>
        /// Removes the first item duplicate of a given item.
        /// </summary>
        private void RemoveDuplicate(T item)
        {
            bool returneDefault;
            T existing = items.FirstOrDefault(i => Comparison(i, item) == 0, default, out returneDefault);

            if (!returneDefault)
                items.Remove(existing);
        }

        /// <inheritdoc/>
        public void Clear() => items.Clear();

        /// <inheritdoc/>
        public bool Contains(T item) => items.Contains(item);

        /// <inheritdoc/>m>
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        /// <inheritdoc/>>
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        /// <inheritdoc/>
        public int IndexOf(T item) => items.IndexOf(item);

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            if (!items.Any(a => Comparison(a, item) == 0))
                items.Insert(index, item);
        }

        /// <inheritdoc/>
        public bool Remove(T item) => items.Remove(item);
        /// <inheritdoc/>
        public void RemoveAt(int index) => items.RemoveAt(index);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
