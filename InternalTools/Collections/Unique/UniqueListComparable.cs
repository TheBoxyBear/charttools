using System;
using System.Collections.Generic;

namespace ChartTools.Collections
{
    /// <summary>
    /// List where all <typeparamref name="T"/> items must be unique using the default <see cref="IComparable{T}"/> comparison to determine uniqueness
    /// </summary>
    public class UniqueListComparable<T> : UniqueList<T> where T : IComparable<T>
    {
        /// <summary>
        /// Creates an instance of <see cref="UniqueListComparable{T}"/>.
        /// </summary>
        public UniqueListComparable(int capacity = 0, IEnumerable<T> items = null) : base((T a, T b) => a.CompareTo(b), capacity, items) { }
    }
}
