using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerator that yields <typeparamref name="T"/> items by alternating through a set of enumerators
    /// </summary>
    /// <typeparam name="T">Type of the enumerated items</typeparam>
    public class SerialAlternatingEnumerator<T> : IEnumerator<T>, IInitializable
    {
        /// <summary>
        /// Enumerators to alternate between
        /// </summary>
        private IEnumerator<T>[] Enumerators { get; }
        /// <summary>
        /// Position of the next enumerator to pull from
        /// </summary>
        private int index;

        /// <summary>
        /// Item to use in the iteration
        /// </summary>
        public T Current { get; private set; }
        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <summary>
        /// True if the enumerator is initialized and the current item can be retrieved.
        /// </summary>
        public bool Initialized { get; private set; }
        /// <summary>
        /// <see langword="true"/> for indexes where MoveNext previously returned <see langword="false"/>
        /// </summary>
        private bool[] endsReached;

        /// <summary>
        /// Creates an instance of <see cref="SerialAlternatingEnumerator{T}"/>
        /// </summary>
        /// <param name="enumerators">Enumerators to alternate between</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public SerialAlternatingEnumerator(params IEnumerator<T>[] enumerators)
        {
            if (enumerators is null)
                throw new CommonExceptions.ParameterNullException("enumerators", 1);
            if (enumerators.Length == 0)
                throw new ArgumentException("No enumerators provided.");

            Enumerators = enumerators.Where(e => e is not null).ToArray();
            endsReached = new bool[Enumerators.Length];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Dispose();
        }
        ~SerialAlternatingEnumerator() => Dispose();

        /// <inheritdoc/>
        public bool MoveNext()
        {
            int startingIndex = index;
            bool startingPassed = false;

            Initialize();
            Enumerators[index].MoveNext();

            return SearchEnumerator();

            bool SearchEnumerator()
            {
                // Skip enumerator if ended
                if (endsReached[index])
                    if (++index == Enumerators.Length)
                        return false;

                IEnumerator<T> enumerator = Enumerators[index];

                try { Current = enumerator.Current; }
                catch
                {
                    // If end reached, repeat with next enumerator
                    if (!enumerator.MoveNext())
                    {
                        endsReached[index] = true;

                        // First iteration or has looped back around
                        if (index == startingIndex)
                        {
                            // Has looped around, all enumerators have been searched
                            if (startingPassed)
                                return false;

                            startingPassed = true;
                        }

                        // If last enumerator is beaing search, return to the first one
                        if (++index == Enumerators.Length)
                            index = 0;

                        return SearchEnumerator();
                    }
                }

                // Continue search with the same enumerator until it runs out of items or the item is not null
                return Current is not null || enumerator.MoveNext() && SearchEnumerator();
            }
        }

        /// <inheritdoc/>
        public void Initialize()
        {
            if (!Initialized)
            {
                foreach (IEnumerator<T> enumerator in Enumerators)
                    enumerator.MoveNext();

                Initialized = true;
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            // Reset every enumerator
            foreach (IEnumerator<T> enumerator in Enumerators)
                try { enumerator.Reset(); }
                catch { throw; }

            index = 0;
            Initialized = false;
            endsReached = default;
        }
    }
}
