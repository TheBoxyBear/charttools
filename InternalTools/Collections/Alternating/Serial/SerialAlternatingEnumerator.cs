using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerator that yields <typeparamref name="T"/> items by alternating through a set of enumerators
    /// </summary>
    public class SerialAlternatingEnumerator<T> : IAlternatingEnumerator<T>
    {
        /// <summary>
        /// Enumerators to alternate between
        /// </summary>
        public IEnumerator<T>[] Enumerators { get; }
        /// <summary>
        /// Position of the next enumerator to pull from
        /// </summary>
        private int index;

        /// <summary>
        /// Item to use in the iteration
        /// </summary>
        public T Current { get; protected set; }
        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <summary>
        /// True if the enumerator is initialized and the current item can be retrieved.
        /// </summary>
        public bool Initialized { get; private set; }

        /// <summary>
        /// Creates an instance of <see cref="SerialAlternatingEnumerator{T}"/>
        /// </summary>
        /// <param name="enumerators">Enumerators to alternate between</param>
        /// <exception cref="ArgumentException"/>
        public SerialAlternatingEnumerator(params IEnumerator<T>[] enumerators)
        {
            if (enumerators is null)
                throw new ArgumentException("No enumerators provided.");

            Enumerators = enumerators.Where(e => e is not null).ToArray();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Dispose();
        }
        ~SerialAlternatingEnumerator() => Dispose();

        /// <inheritdoc/>
        public virtual bool MoveNext()
        {
            //Find the first non-null enumerator
            while (Enumerators[index] is null)
                index++;

            //Return false is none found
            if (index == Enumerators.Length)
            {
                index = 0;
                return false;
            }

            //Move enumerator to the first non-null item
            while (Enumerators[index].Current is null)
                if (!Enumerators[index].MoveNext())
                    return MoveNext();

            Current = Enumerators[index].Current;
            return true;
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

        /// <summary>
        /// Resets the enumerator to the first item.
        /// </summary>
        public virtual void Reset()
        {
            //Reset every enumerator
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Reset();

            index = 0;
            Initialized = false;
        }
    }
}
