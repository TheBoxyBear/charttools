using ChartTools.Internal;
using ChartTools.Internal.Collections.Delayed;
using ChartTools.IO.Chart.Parsers;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Reader of text file that sends read lines to subscribers of its events.
    /// </summary>
    internal class ChartFileReader
    {
        private record ParserLinesGroup(ChartParser Parser, DelayedEnumerableSource<string> Source);

        public string Path { get; }
        public IEnumerable<ChartParser> Parsers => parserGroups.Select(g => g.Parser);

        private readonly List<ParserLinesGroup> parserGroups = new();
        private readonly List<Task> parseTasks = new();
        private readonly Func<string, ChartParser?> parserGetter;
        private const string partEndEarlyExceptionMessage = "Part \"{0}\" did not end within the provided lines.";

        public ChartFileReader(string path, Func<string, ChartParser?> parserGetter)
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

                // Move past the part name and opening bracket
                for (int i = 0; i < 2; i++)
                    if (!enumerator.MoveNext())
                        return;

                // Read until closing bracket
                while (!ChartFormatting.IsSectionEnd(enumerator.Current))
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
            var enumerator = AsyncFileReader.ReadFileAsync(Path).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).GetAsyncEnumerator();
            var readTask = enumerator.MoveNextAsync();
            string? currentLine;

            do
            {
                // Find part
                do
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    await readTask;

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

                // Move past the part name and opening bracket
                if (!await readTask || !await enumerator.MoveNextAsync() || cancellationToken.IsCancellationRequested)
                    return;

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
                while (currentLine != "}");

                if (currentGroup is not null)
                    currentGroup!.Source.EndAwait();
            }
            while (await readTask);

            await enumerator.DisposeAsync();
            await Task.WhenAll(parseTasks);
        }
    }
}
