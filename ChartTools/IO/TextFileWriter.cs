using ChartTools.Collections;
using ChartTools.Extensions.Linq;
using ChartTools.Internal.Collections;
using ChartTools.IO.Sources;

namespace ChartTools.IO;

internal abstract class TextFileWriter(WritingDataSource source, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers)
{
    public WritingDataSource Source { get; } = source;

    protected virtual string? PreSerializerContent => null;
    protected virtual string? PostSerializerContent => null;

    private readonly List<Serializer<string>> serializers = [..serializers];
    private readonly IEnumerable<string>? removedHeaders = removedHeaders;

    private IEnumerable<string> Wrap(string header, IEnumerable<string> lines)
    {
        yield return header;

        if (PreSerializerContent is not null)
            yield return PreSerializerContent;

        foreach (var line in lines)
            yield return line;

        if (PostSerializerContent is not null)
            yield return PostSerializerContent;
    }

    public void Write()
    {
        foreach (var serializer in serializers)
            serializer.Serialize();

        using StreamWriter writer = new(Source.Stream, leaveOpen: true);

        foreach (var line in GetLinesToWrite(serializer => serializer.Serialize()))
            writer.WriteLine(line);

        EndFile();
    }

    public async Task WriteAsync(CancellationToken cancellationToken)
    {
        using var writer = new StreamWriter(Source.Stream, leaveOpen: true);
        var serializerResults = serializers.ToDictionary(ser => ser, ser => new EagerEnumerable<string>(ser.SerializeAsync()));

        foreach (var line in GetLinesToWrite(ser => serializerResults[ser]))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            await writer.WriteLineAsync(line);
        }

        EndFile();
    }

    private void EndFile() => Source.Stream.SetLength(Source.Stream.Position);

    private List<string>? GetExistingLines()
    {
        List<string>? lines = null;

        if (Source.Existing is not null)
        {
            lines = [];
            string? line = null;

            using StreamReader reader = new(Source.Existing.Stream, leaveOpen: true);
            string content = reader.ReadToEnd();

            while ((line = reader.ReadLine()) is not null)
                lines.Add(line);
        }

        return lines;
    }

    private IEnumerable<string> GetLinesToWrite(Func<Serializer<string>, IEnumerable<string>> getSerializerLines)
    {
        // Using the reader stream can modify the position of the write stream if both are connected
        var initialWriterPosition = Source.Stream.Position;
        var existing = GetExistingLines();

        Source.Stream.Position = initialWriterPosition;

        if (existing?.Count > 0)
        {
            var replacements = from serializer in serializers
                               select new SectionReplacement<string>(
                                   Wrap(serializer.Header, getSerializerLines(serializer)),
                                   line => line == serializer.Header, EndReplace, true);

            if (removedHeaders is not null)
                replacements = replacements.Concat(removedHeaders.Select(header => new SectionReplacement<string>([], line => line == header, EndReplace, false)));

            return existing.ReplaceSections(replacements);
        }
        else
            return serializers.SelectMany(serializer => Wrap(serializer.Header, getSerializerLines(serializer)));
    }

    protected abstract bool EndReplace(string line);
}
