using System.Collections.Concurrent;

namespace ChartTools.Internal.Collections.Delayed
{
    public class DelayedEnumerableSource<T>
    {
        public ConcurrentQueue<T> Buffer { get; } = new();
        public DelayedEnumerable<T> Enumerable { get; }
        public bool AwaitingItems { get; private set; }

        public DelayedEnumerableSource() => Enumerable = new(this);

        public void Add(T item) => Buffer.Enqueue(item);
        public void EndAwait() => AwaitingItems = false;
    }
}
