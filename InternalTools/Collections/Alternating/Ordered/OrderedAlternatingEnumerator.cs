using System;
using System.Collections;
using System.Collections.Generic;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerator that yields <typeparamref name="T"/> items from a set of enumerators in order using a <typeparamref name="TKey"/> key
    /// </summary>
    public class OrderedAlternatingEnumerator<T, TKey> : IAlternatingEnumerator<T> where TKey : IComparable<TKey>
    {
        /// <inheritdoc/>
        public IEnumerator<T>[] Enumerators { get; }
        /// <summary>
        /// Method that retrieves the key from an item
        /// </summary>
        public Func<T, TKey> KeyGetter { get; set; }
        /// <inheritdoc/>
        public bool Initialized { get; private set; }

        /// <inheritdoc/>
        public T Current { get; private set; }
        /// <inheritdoc/>
        object IEnumerator.Current => Current;

        /// <summary>
        /// State of the enumerators. True where MoveNext previously returned false
        /// </summary>
        private bool[] endsReached;

        /// <summary>
        /// Creates a new instance of <see cref="OrderedAlternatingEnumerator{T, TKey}"/>.
        /// </summary>
        public OrderedAlternatingEnumerator(Func<T, TKey> keyGetter, params IEnumerator<T>[] enumerators)
        {
            Enumerators = enumerators;
            KeyGetter = keyGetter;
            endsReached = new bool[Enumerators.Length];
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Dispose();
        }
        ~OrderedAlternatingEnumerator() => Dispose();

        /// <inheritdoc/>
        public bool MoveNext()
        {
            TKey smallestKey = default;
            int index = 0;
            int smallestKeyIndex = -1;

            Initialize();

            foreach (IEnumerator<T> enumerator in Enumerators)
            {
                //Skip ended enumerators
                if (endsReached[index])
                {
                    index++;
                    continue;
                }

                //Set current to next non-null
                while (enumerator.Current is null)
                    if (!enumerator.MoveNext())
                    {
                        endsReached[index] = true;
                        break;
                    }

                //Skip if ended
                if (endsReached[index])
                {
                    index++;
                    continue;
                }

                TKey key = KeyGetter(enumerator.Current);

                //Assign first key
                if (smallestKeyIndex == -1)
                {
                    smallestKey = key;
                    smallestKeyIndex = index;
                }
                //Assign new smallest key
                else if (key.CompareTo(smallestKey) == -1)
                {
                    smallestKey = key;
                    smallestKeyIndex = index;
                }

                index++;
            }

            //No key
            if (smallestKeyIndex == -1)
                return false;

            Current = Enumerators[smallestKeyIndex].Current;

            //Move selected enumerator to the next item
            if (!Enumerators[smallestKeyIndex].MoveNext())
                endsReached[smallestKeyIndex] = true;

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
        public void Reset()
        {
            for (int i = 0; i < endsReached.Length; i++)
                endsReached[i] = false;
        }
    }
}
