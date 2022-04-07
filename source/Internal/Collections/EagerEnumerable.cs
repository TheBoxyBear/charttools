using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.Internal.Collections
{
    internal class EagerEnumerable<T> : IEnumerable<T>
    {
        private IEnumerable<T> items;
        private readonly Task<IEnumerable<T>> source;

        public EagerEnumerable(Task<IEnumerable<T>> source) => this.source = source;

        public IEnumerator<T> GetEnumerator()
        {
            if (items is null)
            {
                source.Wait();
                items = source.Result;
            }

            return items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
