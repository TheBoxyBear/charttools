using System.Collections.Generic;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Defines an enumerable composed of a set of enumerables of <typeparamref name="T"/> items
    /// </summary>
    public interface IAlternatingEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Enumerables to pull items from
        /// </summary>
        public IEnumerable<T>[] Enumerables { get; }
    }
}
