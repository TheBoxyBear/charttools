using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChartTools.IO.Serializaiton
{
    internal abstract class Serializer<TResult>
    {
        protected WritingSession session;

        public string Header { get; }

        public Serializer(string header, WritingSession session)
        {
            Header = header;
            this.session = session;
        }

        public abstract IEnumerable<TResult> Serialize();
        public async Task<IEnumerable<TResult>> SerializeAsync() => await Task.Run(() => Serialize().ToArray());
    }

    internal abstract class Serializer<TContent, TResult> : Serializer<TResult>
    {
        public TContent Content { get; }

        public Serializer(string header, TContent content, WritingSession session) : base(header, session) => Content = content;
    }
}
