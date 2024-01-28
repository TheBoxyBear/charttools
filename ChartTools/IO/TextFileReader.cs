using ChartTools.Extensions.Collections;
using ChartTools.IO.Parsing;

namespace ChartTools.IO;

internal abstract class TextFileReader(TextReader reader, Func<string, TextParser?> parserGetter) : FileReader<string, TextParser>(parserGetter)
{
    public TextReader Reader { get; } = reader;
    public virtual bool DefinedSectionEnd { get; } = false;

    public TextFileReader(Stream stream, Func<string, TextParser?> parserGetter) : this(new StreamReader(stream), parserGetter) { }

    public TextFileReader(string path, Func<string, TextParser?> parserGetter) : this(new FileStream(path, FileMode.Open), parserGetter) => ownedResources.Add(Reader);

    protected override void ReadBase(bool async, CancellationToken cancellationToken)
    {
        ParserContentGroup? currentGroup = null;
        string line = string.Empty;

        while (ReadLine())
        {
            // Find part
            while (!line.StartsWith('['))
                if (!ReadLine())
                    return;

            if (async && cancellationToken.IsCancellationRequested)
            {
                Dispose();
                return;
            }

            var header = line;
            var parser = parserGetter(header);

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
                    Finish();
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
                    Finish();
                    return;
                }
            }

            Finish();

            void Finish()
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

            while ((newLine = Reader.ReadLine()) == string.Empty) ;

            if (newLine is null)
                return false;

            line = newLine.Trim();
            return true;
        }
    }

    protected abstract bool IsSectionStart(string line);
    protected virtual bool IsSectionEnd(string line) => false;
}
