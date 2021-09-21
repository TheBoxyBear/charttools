using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Sorted
{
    /// <summary>
    /// *Deprecated* Collection where <typeparamref name="TValue"/> items are always sorted based on a <typeparamref name="TKey"/> key
    /// </summary>
    /// <typeparam name="TKey">Type of the key to use as in index</typeparam>
    /// <typeparam name="TValue">Type of the contained items</typeparam>
    [Obsolete("Unused by non-internal classes")]
    internal class IndexableSelfSorted<TKey, TValue> : SelfSorted<TValue> where TKey : IComparable<TKey> where TValue : IComparable<TValue>
    {
        /// <summary>
        /// Method that retrieves the key from an item
        /// </summary>
        protected Func<TValue, TKey> GetKey;
        /// <summary>
        /// <see langword="true"/> if multiple items can have the same key. Items of the same key are sorted using the default <see cref="IComparable{T}"/> comparison of <typeparamref name="TValue"/>.
        /// </summary>
        public bool AllowDuplicates { get; set; } = false;
        /// <summary>
        /// Gets the number of unique keys from the items
        /// </summary>
        public int KeyCount => this.GroupBy(t => GetKey(t)).Count();

        /// <summary>
        /// Creates an instance of <see cref="IndexableSelfSorted{TKey, TValue}"/>.
        /// </summary>
        /// <param name="keyGetter">Method that retrieves the key from an item</param>
        /// <param name="capacity">Number of items that the <see cref="IndexableSelfSorted{TKey, TValue}"/> can initially store</param>
        public IndexableSelfSorted(Func<TValue, TKey> keyGetter, int capacity = 0) : base(capacity) => GetKey = keyGetter;

        /// <summary>
        /// Gets the items matching a key.
        /// </summary>
        /// <param name="key">Key to match the items against</param>
        private IEnumerable<TValue> GetValues(TKey key) => items.Where(i => GetKey(i).Equals(key));

        /// <inheritdoc/>
        /// <exception cref="ArgumentException"/>
        public override void Add(TValue item)
        {
            if (AllowDuplicates)
                base.Add(item);
            else
                if (ContainsDuplicate(item))
                throw new ArgumentException("Item cannot be added because one of the same index already exists.");
            else
                base.Add(item);
        }
        /// <inheritdoc/>
        /// <param name="items">Items to add</param>
        /// <exception cref="ArgumentException"/>
        public override void AddRange(IEnumerable<TValue> items)
        {
            foreach (TValue i in items)
                if (ContainsDuplicate(i))
                    throw new ArgumentException("One or more items cannot be added because some with the same index already exist.");

            base.AddRange(items);
        }
        /// <summary>
        /// Determines if the collection contains any item of the same key as the provided item.
        /// </summary>
        /// <param name="item">Item to search for duplicates of</param>
        private bool ContainsDuplicate(TValue item)
        {
            foreach (TValue i in items)
                switch (GetKey(i).CompareTo(GetKey(item)))
                {
                    case 0:
                        return true;
                    case 1:
                        return false;
                }
            return false;
        }

        /// <summary>
        /// Gets or replaces the items matching a key.
        /// </summary>
        /// <param name="key">Key to match the items against</param>
        /// <returns></returns>
        public IEnumerable<TValue> this[TKey key]
        {
            get => GetValues(key);
            set
            {
                RemoveAt(key);
                AddRange(value);
            }
        }

        /// <summary>Removes the items matching a key.</summary>
        /// <param name="key">Key of items to remove</param>
        public void RemoveAt(TKey key)
        {
            foreach (TValue v in GetValues(key))
                Remove(v);
        }
    }
}
