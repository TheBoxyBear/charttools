using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerable where <typeparamref name="T"/> items are yielded by alternating from a set of enumerables
    /// </summary>
    /// <typeparam name="T">Type of the enumerated items</typeparam>
    public class SerialAlternatingEnumerable<T> : IEnumerable<T>
    {
        /// <inheritdoc/>
        protected IEnumerable<T>[] Enumerables { get; }

        /// <summary>
        /// Creates an instance of <see cref="SerialAlternatingEnumerable{T}"/>
        /// </summary>
        /// <param name="enumerables">Enumerables to pull items from</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public SerialAlternatingEnumerable(params IEnumerable<T>?[] enumerables)
        {
            if (enumerables is null)
                throw new ArgumentNullException(nameof(enumerables));
            if (enumerables.Length == 0)
                throw new ArgumentException("No enumerables provided.");

            Enumerables = enumerables.NonNull().ToArray();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new SerialAlternatingEnumerator<T>(Enumerables.Select(e => e.GetEnumerator()).ToArray());
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
