using System.Collections;

using ChartTools.SystemExtensions;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Unique
{
    /// <summary>
    /// Enumerable where <typeparamref name="T"/> items are pulled from multiple enumerables and filtered to the ones considered unique by an <see cref="EqualityComparison{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of the enumerated items</typeparam>
    [Obsolete("Use SelectMany().Distinct(EqualityComparison<T>) instead")]
    public class UniqueEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Enumerables to pull items from
        /// </summary>
        public IEnumerable<T>[] Enumerables { get; }
        /// <summary>
        /// Function that determines if two items are the same
        /// </summary>
        private EqualityComparison<T> Comparison { get; }

        /// <summary>
        /// Creates an instance of <see cref="UniqueEnumerable{T}"/>.
        /// </summary>
        /// <param name="comparison">Function that determines if two items are the same</param>
        /// <param name="enumerables">Enumerables to pull items from</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public UniqueEnumerable(EqualityComparison<T> comparison, params IEnumerable<T>?[] enumerables)
        {
            if (comparison is null)
                throw new ArgumentNullException(nameof(comparison));
            if (enumerables is null)
                throw new ArgumentNullException(nameof(enumerables));
            if (enumerables.Length == 0)
                throw new ArgumentException("No enumerables provided.");

            Comparison = comparison;
            Enumerables = enumerables.NonNull().ToArray();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new UniqueEnumerator<T>(Comparison, Enumerables.Select(e => e.GetEnumerator()).ToArray());
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
