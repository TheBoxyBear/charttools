using ChartTools.Internal.Collections.Delayed;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class Serializer<T>
    {
        public string Header { get; }

        protected List<T>? preResult;
        protected DelayedEnumerableSource<string> outputSource = new();
        protected WritingSession? session;

        public Serializer(string header, WritingSession session)
        {
            Header = header;
            this.session = session;
        }

        public abstract IEnumerable<string> Serialize();
        public async Task<IEnumerable<string>> SerializeAsync()
        {
            DelayedEnumerableSource<string> linesSource = new();

            await Task.Run(() =>
            {
                foreach (var line in Serialize())
                    linesSource.Add(line);
            });

            return linesSource.Enumerable;
        }

        internal static string GetLine(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";
    }
}
