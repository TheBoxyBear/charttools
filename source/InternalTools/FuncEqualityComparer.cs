using System;
using System.Collections.Generic;

using ChartTools.SystemExtensions;

namespace ChartTools
{
    /// <summary>
    /// Delegate-based <see cref="IEqualityComparer{T}"/>
    /// </summary>
    public class FuncEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// Method used to compare two objects
        /// </summary>
        public EqualityComparison<T?> Comparison { get; }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="comparison">Method used to compare two objects</param>
        public FuncEqualityComparer(EqualityComparison<T?> comparison)
        {
            if (comparison is null)
                throw new ArgumentNullException(nameof(comparison), "The comparison is null");

            Comparison = comparison;
        }

        public bool Equals(T? x, T? y) => Comparison(x, y);
        public int GetHashCode(T obj) => obj!.GetHashCode();
    }
}
