using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerable where <typeparamref name="T"/> items are pulled from a set of enumerables in order using a <typeparamref name="TKey"/> key
    /// </summary>
    public class OrderedAlternatingEnumerable<T, TKey> : IAlternatingEnumerable<T> where TKey : IComparable<TKey>
    {
        ///<inheritdoc/>
        public IEnumerable<T>[] Enumerables { get; }
        /// <summary>
        /// Method that retrieves the key from an item
        /// </summary>
        internal Func<T, TKey> KeyGetter { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="OrderedAlternatingEnumerable{T, TKey}"/>.
        /// </summary>
        /// <param name="keyGetter">Method that retrieves the key from an item</param>
        /// <param name="enumerables">Enumerables to pull items from</param>
        public OrderedAlternatingEnumerable(Func<T, TKey> keyGetter, params IEnumerable<T>[] enumerables)
        {
            KeyGetter = keyGetter;
            Enumerables = enumerables;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new OrderedAlternatingEnumerator<T, TKey>(KeyGetter, Enumerables.Where(e => e is not null).Select(e => e.GetEnumerator()).ToArray());
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
