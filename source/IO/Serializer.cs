using ChartTools.Internal.Collections.Delayed;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class Serializer<T>
    {
        public string Header { get; }
        public bool ResultReady { get; private set; }

        protected List<T>? preResult;
        protected DelayedEnumerableSource<T> outputSource = new();
        protected WritingSession? session;

        public Serializer(string header, WritingSession session)
        {
            Header = header;
            this.session = session;
        }

        public abstract IEnumerable<T> Serialize();
        public async Task<IEnumerable<T>> SerializeAsync()
        {
            await Task.Run(() =>
            {
                foreach (var item in Serialize())
                    outputSource.Add(item);
            });

            return outputSource.Enumerable;
        }
    }
}
