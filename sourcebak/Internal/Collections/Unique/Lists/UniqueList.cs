using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Unique
{
    /// <summary>
    /// List where all <typeparamref name="T"/> items must be unique using a given comparison
    /// </summary>
    /// <typeparam name="T">Type of the contained items</typeparam>
    public class UniqueList<T> : IList<T>
    {
        /// <summary>
        /// Source of items
        /// </summary>
        protected List<T> items;
        /// <summary>
        /// Method that defines uniqueness of items
        /// </summary>
        protected EqualityComparison<T> Comparison { get; }

        /// <summary>
        /// Creates an instance of <see cref="UniqueList{T}"/> using a comparison to define the uniqueness of items.
        /// </summary>
        /// <param name="comparison">Method that defines uniqueness of items</param>
        /// <param name="capacity">Number of items that the <see cref="UniqueList{T}"/> can initially store</param>
        public UniqueList(EqualityComparison<T> comparison, int capacity = 0, IEnumerable<T?>? items = null)
        {
            if (comparison is null)
                throw new ArgumentNullException(nameof(comparison));

            Comparison = comparison;
            this.items = new List<T>(capacity);

            if (items is not null)
                AddRange(items!);
        }
        /// <inheritdoc/>
        public T this[int index]
        {
            get => items[index];
            set
            {
                RemoveDuplicate(value);
                items[index] = value;
            }
        }

        public int Count => items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// Adds an item to the list and overwrites any duplicate.
        /// </summary>
        /// <param name="item">Item to add</param>
        public virtual void Add(T item)
        {
            RemoveDuplicate(item);
            items.Add(item);
        }

        /// <summary>
        /// Adds multiple items to the <see cref="ICollection{T}"/>
        /// </summary>
        /// <param name="collection">Items to add</param>
        public virtual void AddRange(IEnumerable<T> collection)
        {
            var itemList = new List<T>(collection.NonNull().Distinct(Comparison!));

            RemoveDuplicates(itemList);
            items.AddRange(itemList);
        }

        /// <summary>
        /// Removes the first duplicate of a given item.
        /// </summary>
        /// <param name="item">Item to remove the duplicate of</param>
        private void RemoveDuplicate(T item)
        {
            if (items.TryGetFirst(i => Comparison(i, item), out T? existing))
                items.Remove(existing!);
        }
        /// <summary>
        /// Removes the first duplicate of multiple items
        /// </summary>
        /// <param name="item">Items to remove the duplicate of</param>
        private void RemoveDuplicates(List<T> collection)
        {
            int i = 0;

            while (i < items.Count)
            {
                if (collection.Count == 0)
                    return;

                for (int j = 0; j < collection.Count; j++)
                    if (Comparison(collection[j], items[i]))
                    {
                        items.RemoveAt(i);
                        collection.RemoveAt(j);
                    }
                    else
                        i++;
            }
        }

        /// <inheritdoc/>
        public void Clear() => items.Clear();

        /// <inheritdoc/>
        public bool Contains(T item) => items.Contains(item);

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex) => items.CopyTo(array, arrayIndex);

        /// <inheritdoc/>>
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();

        /// <inheritdoc/>
        public int IndexOf(T item) => items.IndexOf(item);

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            RemoveDuplicate(item);
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
