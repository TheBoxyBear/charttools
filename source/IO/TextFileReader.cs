using ChartTools.Internal;
using ChartTools.Internal.Collections.Delayed;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO
{
    internal abstract class TextFileReader
    {
        private record ParserLinesGroup(FileParser<string> Parser, DelayedEnumerableSource<string> Source);

        public string Path { get; }
        public IEnumerable<FileParser<string>> Parsers => parserGroups.Select(g => g.Parser);

        private readonly List<ParserLinesGroup> parserGroups = new();
        private readonly List<Task> parseTasks = new();
        private readonly Func<string, FileParser<string>?> parserGetter;
        private const string partEndEarlyExceptionMessage = "Part \"{0}\" did not end within the provided lines.";

        public TextFileReader(string path, Func<string, FileParser<string>?> parserGetter)
        {
            Path = path;
            this.parserGetter = parserGetter;
        }

        public void Read()
        {
            ParserLinesGroup? currentGroup = null;
            using var enumerator = File.ReadLines(Path).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).GetEnumerator();
            var partLines = new List<string>();

            while (enumerator.MoveNext())
            {
                // Find part
                while (!enumerator.Current.StartsWith('['))
                    if (enumerator.MoveNext())
                        return;

                string header = enumerator.Current;

                var parser = parserGetter(header);

                if (parser is not null)
                    parserGroups.Add(currentGroup = new(parser, new()));

                // Move to the start of the entries
                while (enumerator.MoveNext() && !IsSectionStart(enumerator.Current)) { }

                // Read until end
                while (!IsSectionEnd(enumerator.Current))
                {
                    currentGroup?.Source.Add(enumerator.Current);

                    if (!enumerator.MoveNext())
                        throw new InvalidDataException(string.Format(partEndEarlyExceptionMessage, header));
                }
            }

            foreach (var group in parserGroups)
                group.Parser.Parse(group.Source.Enumerable);
        }
        public async Task ReadAsync(CancellationToken cancellationToken)
        {
            ParserLinesGroup? currentGroup = null;
            var enumerator = AsyncFileReader.ReadFileAsync(Path).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).GetAsyncEnumerator(cancellationToken);
            var readTask = enumerator.MoveNextAsync();
            string? currentLine;

            do
            {
                // Find part
                do
                {
                    if (cancellationToken.IsCancellationRequested || !await readTask)
                        return;

                    currentLine = enumerator.Current;
                    readTask = enumerator.MoveNextAsync();
                }
                while (!currentLine.StartsWith('['));

                string header = currentLine;
                var parser = parserGetter(header);

                if (parser is not null)
                {
                    var source = new DelayedEnumerableSource<string>();

                    parserGroups.Add(currentGroup = new(parser, source));
                    parseTasks.Add(parser.StartAsyncParse(source.Enumerable));
                }

                // Move to the first entry
                do
                {
                    if (cancellationToken.IsCancellationRequested || !await readTask)
                        return;

                    currentLine = enumerator.Current;
                    readTask = enumerator.MoveNextAsync();
                }
                while (!IsSectionStart(currentLine));

                currentLine = enumerator.Current;
                readTask = enumerator.MoveNextAsync();

                // Read until closing bracket
                do
                {
                    if (currentGroup is not null)
                        currentGroup.Source.Add(currentLine);

                    if (!await readTask || cancellationToken.IsCancellationRequested)
                        throw new InvalidDataException(string.Format(partEndEarlyExceptionMessage, header));

                    currentLine = enumerator.Current;
                    readTask = enumerator.MoveNextAsync();
                }
                while (!IsSectionEnd(currentLine));

                if (currentGroup is not null)
                    currentGroup!.Source.EndAwait();
            }
            while (await readTask);

            await enumerator.DisposeAsync();
            await Task.WhenAll(parseTasks);
        }

        protected abstract bool IsSectionStart(string line);
        protected abstract bool IsSectionEnd(string line);
    }
}
