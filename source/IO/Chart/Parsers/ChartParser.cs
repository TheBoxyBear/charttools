using ChartTools.IO.Chart.Sessions;

using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Threading.Tasks;

namespace ChartTools.IO.Chart.Parsers
{
    internal abstract class ChartParser
    {
        public abstract object? Result { get; }

        protected ConcurrentQueue<string> buffer = new();
        protected ReadingSession? session;

        public void AddLine(string line) => buffer.Enqueue(line);

        public async Task StartAsyncParse(IEnumerable<string> lines, ReadingSession session)
        {
            this.session = session;

            PrepareParse();
            await Task.Run(() =>
            {
                foreach (var line in lines)
                    HandleLine(line);
            });
            FinaliseParse();
        }

        public void Parse(IEnumerable<string> lines, ReadingSession session)
        {
            this.session = session;

            PrepareParse();

            foreach (var line in lines)
                HandleLine(line);

            FinaliseParse();
        }

        public abstract void ApplyResultToSong(Song song);

        protected abstract void PrepareParse();
        protected abstract void FinaliseParse();

        protected abstract void HandleLine(string line);
    }
}