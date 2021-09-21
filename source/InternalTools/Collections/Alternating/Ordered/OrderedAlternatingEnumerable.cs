using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerable where <typeparamref name="T"/> items are pulled from a set of enumerables in order using a <typeparamref name="TKey"/> key
    /// </summary>
    /// <typeparam name="T">Type of the enumerated items</typeparam>
    /// <typeparam name="TKey">Type of the key used to determine the order</typeparam>
    public class OrderedAlternatingEnumerable<TKey, T> : IEnumerable<T> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// Enumerables to alternate between
        /// </summary>
        private IEnumerable<T>[] Enumerables { get; }
        /// <summary>
        /// Method that retrieves the key from an item
        /// </summary>
        private Func<T, TKey> KeyGetter { get; }

        /// <summary>
        /// Creates an instance of <see cref="OrderedAlternatingEnumerable{T, TKey}"/>.
        /// </summary>
        /// <param name="keyGetter">Method that retrieves the key from an item</param>
        /// <param name="enumerables">Enumerables to alternate between</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public OrderedAlternatingEnumerable(Func<T, TKey> keyGetter, params IEnumerable<T>?[] enumerables)
        {
            if (keyGetter is null)
                throw new ArgumentNullException(nameof(keyGetter));
            if (enumerables is null)
                throw new ArgumentNullException(nameof(enumerables));
            if (enumerables.Length == 0)
                throw new ArgumentException("No enumerables provided.");

            KeyGetter = keyGetter;
            Enumerables = enumerables.NonNull().ToArray();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new OrderedAlternatingEnumerator<T, TKey>(KeyGetter, Enumerables.Where(e => e is not null).Select(e => e.GetEnumerator()).ToArray());
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
