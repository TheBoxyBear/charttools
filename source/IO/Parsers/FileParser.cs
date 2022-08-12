using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class FileParser<T>
    {
        public bool ResultReady { get; private set; }
        public abstract object? Result { get; }
        protected ReadingSession session;

        public FileParser(ReadingSession session) => this.session = session;

        public async Task StartAsyncParse(IEnumerable<T> items)
        {
            await Task.Run(() => ParseBase(items));

            try { FinaliseParse(); }
            catch (Exception e) { throw GetFinalizeException(e); }
        }
        public void Parse(IEnumerable<T> items)
        {
            ParseBase(items);

            try { FinaliseParse(); }
            catch (Exception e) { throw GetFinalizeException(e); }
        }
        private void ParseBase(IEnumerable<T> items)
        {
            foreach (var item in items)
                try { HandleItem(item); }
                catch (Exception e) { throw GetHandleException(item, e); }
        }

        protected abstract void HandleItem(T item);

        protected virtual void FinaliseParse() => ResultReady = true;

        protected TResult GetResult<TResult>(TResult result) => ResultReady ? result : throw new Exception("Result is not ready.");

        protected abstract Exception GetHandleException(T item, Exception innerException);
        protected abstract Exception GetFinalizeException(Exception innerException);
    }
}
