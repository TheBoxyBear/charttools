using System.Collections.Generic;

namespace ChartTools.Collections
{
    /// <summary>
    /// Enumerator which can be initialized to the first item
    /// </summary>
    public interface IInitializableEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// <see langword="true"/> if is initialized and the current item can be retrieved
        /// </summary>
        public bool Initialized { get; }
        /// <summary>
        /// If <see cref="Initialized"/> is <see langword="false"/>, initializes so that <see cref="IEnumerator{T}.Current"/> can be read.
        /// </summary>
        public void Initialize();
    }
}
