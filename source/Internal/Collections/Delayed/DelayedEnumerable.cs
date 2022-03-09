using System.Collections;
using System.Collections.Generic;

namespace ChartTools.Internal.Collections.Delayed
{
    public class DelayedEnumerable<T> : IEnumerable<T>
    {
        private readonly DelayedEnumerator<T> enumerator;
        private readonly DelayedEnumerableSource<T> source;
        public bool AwaitingItems => source.AwaitingItems;

        internal DelayedEnumerable(DelayedEnumerableSource<T> source)
        {
            this.source = source;
            enumerator = new(source);
        }

        public IEnumerator<T> GetEnumerator() => enumerator;
        IEnumerator IEnumerable.GetEnumerator() => enumerator;
    }
}
