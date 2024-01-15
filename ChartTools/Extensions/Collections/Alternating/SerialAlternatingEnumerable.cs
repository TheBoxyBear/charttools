using ChartTools.Extensions.Linq;

using System.Collections;

namespace ChartTools.Extensions.Collections.Alternating;

/// <summary>
/// Enumerable where <typeparamref name="T"/> items are yielded by alternating from a set of enumerables
/// </summary>
/// <typeparam name="T">Type of the enumerated items</typeparam>
public class SerialAlternatingEnumerable<T> : IEnumerable<T>
{
    /// <inheritdoc/>
    protected IEnumerable<T>[] Enumerables { get; }

    /// <summary>
    /// Creates an instance of <see cref="SerialAlternatingEnumerable{T}"/>
    /// </summary>
    /// <param name="enumerables">Enumerables to pull items from</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    public SerialAlternatingEnumerable(params IEnumerable<T>?[] enumerables)
    {
        ArgumentNullException.ThrowIfNull(enumerables);

        if (enumerables.Length == 0)
            throw new ArgumentException("No enumerables provided.");

        Enumerables = enumerables.NonNull().ToArray();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => new Enumerator(Enumerables.Select(e => e.GetEnumerator()).ToArray())!;
    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerator that yields <typeparamref name="T"/> items by alternating through a set of enumerators
    /// </summary>
    /// <param name="enumerators">Enumerators to alternate between</param>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    private class Enumerator(params IEnumerator<T>[] enumerators) : IEnumerator<T?>
    {
        /// <summary>
        /// Enumerators to alternate between
        /// </summary>
        private IEnumerator<T>[] Enumerators { get; } = enumerators.NonNull().ToArray();
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
        ~Enumerator() => Dispose(false);

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
