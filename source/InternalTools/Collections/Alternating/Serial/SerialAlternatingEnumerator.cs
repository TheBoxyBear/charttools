using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerator that yields <typeparamref name="T"/> items by alternating through a set of enumerators
    /// </summary>
    /// <typeparam name="T">Type of the enumerated items</typeparam>
    public class SerialAlternatingEnumerator<T> : IEnumerator<T>
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
        public T? Current { get; private set; }
        /// <inheritdoc/>
        object? IEnumerator.Current => Current;

        /// <summary>
        /// Creates an instance of <see cref="SerialAlternatingEnumerator{T}"/>
        /// </summary>
        /// <param name="enumerators">Enumerators to alternate between</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public SerialAlternatingEnumerator(params IEnumerator<T>[] enumerators)
        {
            if (enumerators is null)
                throw new ArgumentNullException(nameof(enumerators));
            if (enumerators.Length == 0)
                throw new ArgumentException("No enumerators provided.");

            Enumerators = enumerators.NonNull().ToArray();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        public virtual void Dispose(bool disposing)
        {
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Dispose();
        }
        ~SerialAlternatingEnumerator() => Dispose(false);

        /// <inheritdoc/>
        public bool MoveNext()
        {
            int startingIndex = index;
            return SearchEnumerator();

            bool SearchEnumerator()
            {
                IEnumerator<T> enumerator = Enumerators[index];

                if (enumerator.MoveNext())
                {
                    Current = enumerator.Current;

                    // Move to the next enumerator
                    if (++index == Enumerators.Length)
                        index = 0;

                    return true;
                }

                // End if looped back around to the first enumerator checked, else check the next enumerator
                return index != startingIndex && SearchEnumerator();
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            // Reset every enumerator
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Reset();

            index = 0;
        }
    }
}
