using ChartTools.Extensions.Collections;
using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;

namespace ChartTools.IO;

internal abstract class TextFileReader(ReadingDataSource source) : FileReader<string, TextParser>(source)
{
    public virtual bool DefinedSectionEnd { get; } = false;

    protected bool _disposeReader = false;

    protected override void ReadBase(bool async, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(Source.Stream, leaveOpen: true);

        ParserContentGroup? currentGroup = null;
        string line = string.Empty;

        while (ReadLine())
        {
            // Find section
            while (!line.StartsWith('['))
                if (!ReadLine())
                    return;

            if (async && cancellationToken.IsCancellationRequested)
            {
                Dispose();
                return;
            }

            var header = line;
            var parser = GetParser(header);

            if (parser is not null)
            {
                var source = new DelayedEnumerableSource<string>();

                parserGroups.Add(currentGroup = new(parser, source));

                if (async)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Dispose();
                        return;
                    }

                    parseTasks.Add(parser.StartAsyncParse(source.Enumerable));
                }
            }

            // Move to the start of the entries
            do
                if (!AdvanceSection())
                {
                    FinishSection();
                    return;
                }
            while (!IsSectionStart(line));

            AdvanceSection();

            // Read until end
            while (!IsSectionEnd(line))
            {
                currentGroup?.Source.Add(line);

                if (!AdvanceSection())
                {
                    FinishSection();
                    return;
                }
            }

            FinishSection();

            void FinishSection()
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Dispose();
                    return;
                }

                currentGroup?.Source.EndAwait();
            }

            bool AdvanceSection() => ReadLine() || (DefinedSectionEnd ? throw SectionException.EarlyEnd(header) : false);
        }

        bool ReadLine()
        {
            string? newLine;

            while ((newLine = reader.ReadLine()) == string.Empty) ;

            if (newLine is null)
                return false;

            line = newLine.Trim();
            return true;
        }
    }

    protected abstract bool IsSectionStart(string line);
    protected virtual bool IsSectionEnd(string line) => false;
}
