using ChartTools.Internal.Collections.Delayed;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class TextFileReader : IDisposable
    {
        private record ParserLinesGroup(TextParser Parser, DelayedEnumerableSource<string> Source);

        public string Path { get; }
        public virtual bool DefinedSectionEnd { get; } = false;
        public virtual IEnumerable<TextParser> Parsers => parserGroups.Select(g => g.Parser);

        private readonly List<ParserLinesGroup> parserGroups = new();
        private readonly List<Task> parseTasks = new();
        private readonly Func<string, TextParser?> parserGetter;
        private readonly IEnumerator<string> enumerator;
        private bool disposedValue;

        public TextFileReader(string path, Func<string, TextParser?> parserGetter)
        {
            Path = path;
            this.parserGetter = parserGetter;
            enumerator = File.ReadLines(path).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).GetEnumerator();
        }

        public void Read()
        {
            BaseRead(false, CancellationToken.None);

            foreach (var group in parserGroups)
                group.Parser.Parse(group.Source.Enumerable.EnumerateSynchronously());
        }
        public async Task ReadAsync(CancellationToken cancellationToken)
        {
            BaseRead(true, cancellationToken);
            await Task.WhenAll(parseTasks);
        }
        private void BaseRead(bool async, CancellationToken cancellationToken)
        {
            ParserLinesGroup? currentGroup = null;

            while (enumerator.MoveNext())
            {
                // Find part
                while (!enumerator.Current.StartsWith('['))
                    if (enumerator.MoveNext())
                        return;

                if (async && cancellationToken.IsCancellationRequested)
                {
                    Dispose();
                    return;
                }

                var header = enumerator.Current;
                var parser = parserGetter(header);

                if (parser is not null)
                {
                    var source = new DelayedEnumerableSource<string>();

                    parserGroups.Add(currentGroup = new(parser, source));

                    if (async)
                        parseTasks.Add(parser.StartAsyncParse(source.Enumerable));
                }

                // Move to the start of the entries
                do
                    if (!AdvanceSection())
                    {
                        Finish();
                        return;
                    }
                while (!IsSectionStart(enumerator.Current));

                AdvanceSection();

                // Read until end
                while (!IsSectionEnd(enumerator.Current))
                {
                    currentGroup?.Source.Add(enumerator.Current);

                    if (!AdvanceSection())
                    {
                        Finish();
                        return;
                    }
                }

                Finish();

                void Finish()
                {
                    if (async || cancellationToken.IsCancellationRequested)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            Dispose();
                            return;
                        }
                    }

                    if (currentGroup is not null)
                        currentGroup!.Source.EndAwait();
                }

                bool AdvanceSection() => enumerator.MoveNext() ? true : (DefinedSectionEnd ? throw SectionException.EarlyEnd(header) : false);
            }
        }

        protected abstract bool IsSectionStart(string line);
        protected virtual bool IsSectionEnd(string line) => false;

        public virtual async void Dispose()
        {
            if (!disposedValue)
            {
                enumerator.Dispose();

                foreach (var group in parserGroups)
                    group.Source.Dispose();

                foreach (var task in parseTasks)
                {
                    await task;
                    task.Dispose();
                }

                disposedValue = true;
            }
        }
    }
}
