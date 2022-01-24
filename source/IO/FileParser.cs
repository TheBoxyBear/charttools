using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class FileParser<T>
    {
        protected bool ResultReady { get; private set; }
        public abstract object? Result { get; }
        protected ReadingSession session;

        public FileParser(ReadingSession session) => this.session = session;

        public async Task StartAsyncParse(IEnumerable<T> items)
        {
            await Task.Run(() =>
            {
                foreach (var item in items)
                    HandleItem(item);
            });
            FinaliseParse();
        }

        public void Parse(IEnumerable<T> items)
        {
            foreach (var item in items)
                HandleItem(item);

            FinaliseParse();
        }

        public abstract void ApplyResultToSong(Song song);

        protected virtual void FinaliseParse() => ResultReady = true;

        protected abstract void HandleItem(T item);

        protected TResult GetResult<TResult>(TResult result) => ResultReady ? result : throw new Exception("Result is not ready.");
    }
}
