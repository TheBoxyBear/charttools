using ChartTools.Internal.Collections.Delayed;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class Serializer<TResult>
    {
        protected DelayedEnumerableSource<TResult> outputSource = new();
        protected WritingSession session;

        public string Header { get; }

        public Serializer(string header, WritingSession session)
        {
            Header = header;
            this.session = session;
        }

        public abstract IEnumerable<TResult> Serialize();
        public async Task<IEnumerable<TResult>> SerializeAsync()
        {
            await Task.Run(() =>
            {
                foreach (var item in Serialize())
                    outputSource.Add(item);
            });

            return outputSource.Enumerable;
        }
    }

    internal abstract class Serializer<TContent, TResult> : Serializer<TResult>
    {
        public TContent Content { get; }

        public Serializer(string header, TContent content, WritingSession session) : base(header, session) => Content = content;
    }
}
