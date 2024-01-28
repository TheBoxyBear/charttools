using ChartTools.Extensions.Collections;
using ChartTools.IO.Parsing;

namespace ChartTools.IO;

internal abstract class TextFileReader(TextReader reader, Func<string, TextParser?> parserGetter) : FileReader<string, TextParser>(parserGetter)
{
    public TextReader Reader { get; } = reader;
    public virtual bool DefinedSectionEnd { get; } = false;

    protected TextFileReader(TextReader reader, Func<string, TextParser?> parserGetter, bool ownedReader) : this(reader, parserGetter)
    {
        if (ownedReader)
            ownedResources.Add(reader);
    }

    public TextFileReader(Stream stream, Func<string, TextParser?> parserGetter) : this(new StreamReader(stream), parserGetter, true) { }
    protected TextFileReader(Stream stream, Func<string, TextParser?> parserGetter, bool ownedStream) : this(stream, parserGetter)
    {
        if (ownedStream)
            ownedResources.Add(stream);
    }

    public TextFileReader(string path, Func<string, TextParser?> parserGetter) : this(new FileStream(path, FileMode.Open), parserGetter, true) { }

    protected override void ReadBase(bool async, CancellationToken cancellationToken)
    {
        ParserContentGroup? currentGroup = null;

        using var enumerator = File.ReadLines(Path).Where(s => !string.IsNullOrEmpty(s)).Select(s => s.Trim()).GetEnumerator();

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
                if (cancellationToken.IsCancellationRequested)
                {
                    Dispose();
                    return;
                }

                currentGroup?.Source.EndAwait();
            }

            bool AdvanceSection() => enumerator.MoveNext() || (DefinedSectionEnd ? throw SectionException.EarlyEnd(header) : false);
        }
    }

    protected abstract bool IsSectionStart(string line);
    protected virtual bool IsSectionEnd(string line) => false;
}
