using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerable where <typeparamref name="T"/> items are yielded by alternating from a set of enumerables
    /// </summary>
    public class SerialAlternatingEnumerable<T> : IAlternatingEnumerable<T>
    {
        /// <inheritdoc/>
        public IEnumerable<T>[] Enumerables { get; }

        /// <summary>
        /// Creates an instance of <see cref="SerialAlternatingEnumerable{T}"/>
        /// </summary>
        public SerialAlternatingEnumerable(params IEnumerable<T>[] enumerables) => Enumerables = enumerables;

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => new SerialAlternatingEnumerator<T>(Enumerables.Select(e => e.GetEnumerator()).ToArray());
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
