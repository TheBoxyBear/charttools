using System;
using System.Collections.Generic;

namespace ChartTools.Collections.Unique
{
    /// <summary>
    /// List where all <typeparamref name="T"/> items must be unique using the default <see cref="IEquatable{T}"/> comparison to determine uniqueness
    /// </summary>
    /// <typeparam name="T">Type of the contained items</typeparam>
    public class UniqueListEquatable<T> : UniqueList<T> where T : IEquatable<T>
    {
        /// <summary>
        /// Creates an instance of <see cref="UniqueListEquatable{T}"/>.
        /// </summary>
        /// <param name="capacity">Number of items that the <see cref="UniqueListEquatable{T}"/> can initially store</param>
        public UniqueListEquatable(int capacity = 0, IEnumerable<T?>? items = null) : base((T a, T b) => a.Equals(b), capacity, items) { }
    }
}
