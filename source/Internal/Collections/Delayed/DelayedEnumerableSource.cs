using System;
using System.Collections.Concurrent;

namespace ChartTools.Internal.Collections.Delayed
{
    public class DelayedEnumerableSource<T> : IDisposable
    {
        private bool disposedValue;

        public ConcurrentQueue<T> Buffer { get; } = new();
        public DelayedEnumerable<T> Enumerable { get; }
        public bool AwaitingItems { get; private set; } = true;

        public DelayedEnumerableSource() => Enumerable = new(this);

        public void Add(T item) => Buffer.Enqueue(item);
        public void EndAwait() => AwaitingItems = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    AwaitingItems = false;
                }

                Enumerable.Dispose();;
                disposedValue = true;
            }
        }

        ~DelayedEnumerableSource() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
