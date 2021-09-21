using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Collections.Alternating
{
    /// <summary>
    /// Enumerator that yields <typeparamref name="T"/> items from a set of enumerators in order using a <typeparamref name="TKey"/> key
    /// </summary>
    /// <typeparam name="T">Type of the enumerated items</typeparam>
    /// <typeparam name="TKey">Type of the key used to determine the order</typeparam>
    public class OrderedAlternatingEnumerator<T, TKey> : IInitializable, IEnumerator<T> where TKey : IComparable<TKey>
    {
        private IEnumerator<T>[] Enumerators { get; }
        /// <summary>
        /// Method that retrieves the key from an item
        /// </summary>
        private Func<T, TKey> KeyGetter { get; }
        /// <inheritdoc/>
        public bool Initialized { get; private set; }

        /// <inheritdoc/>
        public T? Current { get; private set; }
        /// <inheritdoc/>
        object? IEnumerator.Current => Current;

        /// <summary>
        /// <see langword="true"/> for indexes where MoveNext previously returned <see langword="false"/>
        /// </summary>
        readonly bool[] endsReached;

        /// <summary>
        /// Creates a new instance of <see cref="OrderedAlternatingEnumerator{T, TKey}"/>.
        /// </summary>
        /// <param name="keyGetter">Method that retrieves the key from an item</param>
        /// <param name="enumerators">Enumerators to alternate between</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public OrderedAlternatingEnumerator(Func<T, TKey> keyGetter, params IEnumerator<T>[] enumerators)
        {
            if (keyGetter is null)
                throw new ArgumentNullException(nameof(keyGetter));
            if (enumerators is null)
                throw new ArgumentNullException(nameof(enumerators));
            if (enumerators.Length == 0)
                throw new ArgumentException("No enumerators provided.");

            Enumerators = enumerators.NonNull().ToArray();
            KeyGetter = keyGetter;
            endsReached = new bool[Enumerators.Length];
        }
        ~OrderedAlternatingEnumerator() => Dispose(false);

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

        /// <inheritdoc/>
        public bool MoveNext()
        {
            // Index of the enumerators with items left
            LinkedList<int> usableEnumerators = new();

            Initialize();

            for (int i = 0; i < Enumerators.Length; i++)
                if (!endsReached[i])
                    usableEnumerators.AddLast(i);

            if (usableEnumerators.Count == 0)
                return false;

            // Get the index of the enumerators whose current item yields the smallest key
            int minIndex = usableEnumerators.MinBy(i => KeyGetter(Enumerators[i].Current));

            // Get the enumerator of this index and set its current item as Current
            IEnumerator<T> minEnumerator = Enumerators[minIndex];
            Current = minEnumerator.Current;

            // Mark the enumerator as having reached its end if the next item can't be pulled
            if (!minEnumerator.MoveNext())
                endsReached[minIndex] = true;

            return true;
        }

        /// <inheritdoc/>
        public bool Initialize()
        {
            if (Initialized)
                return false;

            for (int i = 0; i < Enumerators.Length; i++)
                endsReached[i] = !Enumerators[i].MoveNext();

            return Initialized = true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            foreach (IEnumerator<T> enumerator in Enumerators)
                enumerator.Reset();

            for (int i = 0; i < endsReached.Length; i++)
                endsReached[i] = false;

            Initialized = false;
        }
    }
}
