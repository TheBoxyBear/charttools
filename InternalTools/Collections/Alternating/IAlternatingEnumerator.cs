using System.Collections.Generic;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Defines an enumerator composed of a set of enumerators of <typeparamref name="T"/> items
    /// </summary>
    public interface IAlternatingEnumerator<T> : IInitializableEnumerator<T>
    {
        /// <summary>
        /// Enumerators to alternate between
        /// </summary>
        public IEnumerator<T>[] Enumerators { get; }
    }
}
