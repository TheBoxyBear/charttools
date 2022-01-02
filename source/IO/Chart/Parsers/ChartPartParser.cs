using ChartTools.IO.Chart.Sessions;

using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class ChartPartParser
    {
        public abstract object? Result { get; }

        protected ConcurrentQueue<string> buffer = new();
        private bool noMoreLines = false;
        protected ReadingSession? session;

        public void AddLine(string line) => buffer.Enqueue(line);

        public async Task StartAsyncParse(ReadingSession session)
        {
            this.session = session;

            PrepareParse();
            await Task.Run(HandleLines);
            FinaliseParse();
        }
        public void EndAsyncParse() => noMoreLines = true;

        public void Parse(ReadingSession session)
        {
            this.session = session;
            noMoreLines = true;

            PrepareParse();
            HandleLines();
            FinaliseParse();
        }

        public abstract void ApplyResultToSong(Song song);

        private bool WaitForLines()
        {
            while (buffer.IsEmpty)
                if (noMoreLines)
                    return false;
            return true;
        }
        protected IEnumerable<string> GetLines()
        {
            do
            {
                if (!WaitForLines())
                    yield break;

                int count = 0;

                while (buffer.TryDequeue(out string? line))
                {
                    count++;
                    yield return line;
                }
            }
            while (!noMoreLines);
        }

        private void HandleLines()
        {
            foreach (var line in GetLines())
                HandleLine(line);
        }

        protected abstract void PrepareParse();
        protected abstract void FinaliseParse();

        protected abstract void HandleLine(string line);
    }
}
