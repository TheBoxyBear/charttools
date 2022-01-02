﻿using ChartTools.IO.Chart.Parsers;
using ChartTools.IO.Chart.Sessions;

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
    internal class ChartReader
    {
        public string Path { get; }
        public IReadOnlyList<ChartPartParser> Parsers => parsers;

        private readonly List<ChartPartParser> parsers = new();
        private readonly List<Task> parseTasks = new();
        private ChartPartParser? currentParser;
        private readonly Func<string, ChartPartParser?> parserGetter;
        private const string partEndEarlyExceptionMessage = "Part \"{0}\" did not end within the provided lines.";

        public ChartReader(string path, Func<string, ChartPartParser?> parserGetter)
        {
            Path = path;
            this.parserGetter = parserGetter;
        }

        public void Read(ReadingSession session)
        {
            using var enumerator = ReadFileAsync().ToEnumerable().GetEnumerator();
            var partLines = new List<string>();

            while (enumerator.MoveNext())
            {
                // Find part
                while (!enumerator.Current.StartsWith('['))
                    if (enumerator.MoveNext())
                        return;

                string partName = enumerator.Current;

                if ((currentParser = parserGetter(partName)) is not null)
                    parsers.Add(currentParser);

                // Move past the part name and opening bracket
                for (int i = 0; i < 2; i++)
                    if (!enumerator.MoveNext())
                        return;

                List<string> lines = new();

                // Read until closing bracket
                while (enumerator.Current != "}")
                {
                    currentParser?.AddLine(enumerator.Current);
                    lines.Add(enumerator.Current);

                    if (!enumerator.MoveNext())
                        throw new InvalidDataException(string.Format(partEndEarlyExceptionMessage, partName));
                }
            }

            foreach (var parser in parsers)
                parser.Parse(session);
        }
        public async Task ReadAsync(ReadingSession session, CancellationToken cancellationToken)
        {
            ChartPartParser? currentParser;
            var enumerator = ReadFileAsync().GetAsyncEnumerator();
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

                string partName = currentLine;
                List<string> lines = new();

                if ((currentParser = parserGetter(partName)) is not null)
                {
                    parsers.Add(currentParser);
                    parseTasks.Add(currentParser.StartAsyncParse(session));
                }

                // Move past the part name and opening bracket
                if (!await readTask || !await enumerator.MoveNextAsync() || cancellationToken.IsCancellationRequested)
                    return;

                currentLine = enumerator.Current;
                readTask = enumerator.MoveNextAsync();

                // Read until closing bracket
                do
                {
                    currentParser?.AddLine(currentLine);
                    lines.Add(currentLine);

                    if (!await readTask || cancellationToken.IsCancellationRequested)
                        throw new InvalidDataException(string.Format(partEndEarlyExceptionMessage, partName));

                    currentLine = enumerator.Current;
                    readTask = enumerator.MoveNextAsync();
                }
                while (currentLine != "}");

                currentParser?.EndAsyncParse();
            }
            while (await readTask);

            await enumerator.DisposeAsync();
            await Task.WhenAll(parseTasks);
        }

        /// <summary>
        /// Enumerates the non-empty lines in the file.
        /// </summary>
        private async IAsyncEnumerable<string> ReadFileAsync()
        {
            using StreamReader reader = new(Path);
            string? line = null;

            do
            {
                var readTask = reader.ReadLineAsync();

                if (!string.IsNullOrEmpty(line))
                    yield return line;

                line = (await readTask)?.Trim();
            }
            while (!reader.EndOfStream);

            if (!string.IsNullOrEmpty(line))
                yield return line;
        }
    }
}
