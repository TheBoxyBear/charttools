using ChartTools.SystemExtensions;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Unique
{
    public class UniqueEnumerable<T> : IEnumerable<T>
    {
        public IEnumerable<T>[] Enumerables { get; }
        /// <summary>
        /// Function that determines if two items are the same
        /// </summary>
        private EqualityComparison<T> Comparison { get; }

        /// <summary>
        /// Creates an instance of <see cref="UniqueEnumerable{T}"/>.
        /// </summary>
        /// <param name="comparison">Function that determines if two items are the same</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public UniqueEnumerable(EqualityComparison<T> comparison, params IEnumerable<T>[] enumerables)
        {
            if (comparison is null)
                throw new ArgumentNullException("Comparison is null.");
            if (enumerables is null)
                throw new ArgumentNullException("Enumerable array is null.");
            if (enumerables.Length == 0)
                throw new ArgumentException("No enumerables provided.");

            Comparison = comparison;
            Enumerables = enumerables;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new UniqueEnumerator<T>(Comparison, Enumerables.Select(e => e.GetEnumerator()).ToArray());
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
