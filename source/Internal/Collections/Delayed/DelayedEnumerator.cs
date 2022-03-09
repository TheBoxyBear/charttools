using System;
using System.Collections;
using System.Collections.Generic;

namespace ChartTools.Internal.Collections.Delayed
{
    public class DelayedEnumerator<T> : IEnumerator<T>
    {
        public T Current { get; private set; }
        object IEnumerator.Current => Current;
        public bool AwaitingItems => source.AwaitingItems;

        private readonly DelayedEnumerableSource<T> source;

        internal DelayedEnumerator(DelayedEnumerableSource<T> source) => this.source = source;

        private bool WaitForItems()
        {
            while (source.Buffer.IsEmpty)
                if (!AwaitingItems && source.Buffer.IsEmpty)
                    return false;

            return true;
        }
        public bool MoveNext()
        {
            if (!WaitForItems())
                return false;

            source.Buffer.TryDequeue(out T? item);
            Current = item!;

            return true;
        }

        public void Dispose() => GC.SuppressFinalize(this);

        public void Reset() => throw new InvalidOperationException();
    }
}
