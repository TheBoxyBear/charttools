using ChartTools.IO.Chart.Sessions;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class ChartSerializer
    {
        public string Header { get; }
        public IReadOnlyList<string>? Result { get; private set; }

        protected List<string>? preResult;
        protected WritingSession? session;
        private CancellationToken? cancellationToken;

        public ChartSerializer(string header) => Header = header;

        public void Serialize(WritingSession session)
        {
            this.session = session;

            PrepareSerialize();
            GenerateLines();
            FinaliseSerialize();
        }
        public async Task SerializeAsync(WritingSession session, CancellationToken cancellationToken)
        {
            this.session = session;
            this.cancellationToken = cancellationToken;

            PrepareSerialize();
            await Task.Run(GenerateLines);
            FinaliseSerialize();
        }

        protected virtual void PrepareSerialize() => preResult = new();
        protected virtual void FinaliseSerialize() => Result = preResult;

        protected abstract void GenerateLines();

        internal static string GetLine(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";
    }
}
