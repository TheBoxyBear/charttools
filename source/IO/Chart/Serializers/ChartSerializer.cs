using ChartTools.Internal.Collections.Delayed;
using ChartTools.IO.Chart.Sessions;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class ChartSerializer
    {
        public string Header { get; }

        protected List<string>? preResult;
        protected DelayedEnumerableSource<string> linesSource = new();
        protected WritingSession? session;

        public ChartSerializer(string header) => Header = header;

        public IEnumerable<string> Serialize(WritingSession session)
        {
            this.session = session;
            return GenerateLines();
        }
        public async Task<IEnumerable<string>> SerializeAsync(WritingSession session)
        {
            this.session = session;
            DelayedEnumerableSource<string> linesSource = new();

            await Task.Run(() =>
            {
                foreach (var line in GenerateLines())
                    linesSource.Add(line);
            });

            return linesSource.Enumerable;
        }

        protected abstract IEnumerable<string> GenerateLines();

        internal static string GetLine(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";
    }
}