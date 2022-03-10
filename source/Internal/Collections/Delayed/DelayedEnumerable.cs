using System;
using System.Collections;
using System.Collections.Generic;

namespace ChartTools.Internal.Collections.Delayed
{
    public class DelayedEnumerable<T> : IEnumerable<T>, IDisposable
    {
        private readonly DelayedEnumerator<T> enumerator;
        private readonly DelayedEnumerableSource<T> source;

        /// <summary>
        /// <see langword="true"/> if there are more items to be received
        /// </summary>
        public bool AwaitingItems => source.AwaitingItems;

        internal DelayedEnumerable(DelayedEnumerableSource<T> source)
        {
            this.source = source;
            enumerator = new(source);
        }


        public IEnumerator<T> GetEnumerator() => enumerator;

        IEnumerator IEnumerable.GetEnumerator() => enumerator;


        public void Dispose() => enumerator.Dispose();
    }
}
