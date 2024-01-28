using ChartTools.Extensions.Linq;
using ChartTools.Internal.Collections;

namespace ChartTools.IO;

internal abstract class TextFileWriter(TextWriter writer, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers) : IDisposable
{
    public string? Path { get; }
    public TextWriter Writer { get; } = writer;
    protected virtual string? PreSerializerContent => null;
    protected virtual string? PostSerializerContent => null;

    private readonly List<Serializer<string>> serializers = [..serializers];
    private readonly string tempPath = string.Empty;
    private readonly IEnumerable<string>? removedHeaders = removedHeaders;
    private readonly bool ownedWriter = false;

    public TextFileWriter(Stream stream, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers) : this(new StreamWriter(stream), removedHeaders, serializers) { }

    public TextFileWriter(string path, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers) : this(new FileStream(System.IO.Path.GetTempFileName(), FileMode.OpenOrCreate, FileAccess.Write), removedHeaders, serializers)
    {
        Path = path;
        ownedWriter = true;
        tempPath = ((Writer as StreamWriter)!.BaseStream as FileStream)!.Name;
    }

    private IEnumerable<SectionReplacement<string>> AddRemoveReplacements(IEnumerable<SectionReplacement<string>> replacements) => removedHeaders is null ? replacements : replacements.Concat(removedHeaders.Select(header => new SectionReplacement<string>(Enumerable.Empty<string>(), line => line == header, EndReplace, false)));

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
        foreach (var line in GetLines(serializer => serializer.Serialize()))
            Writer.WriteLine(line);

        if (Path is not null)
        {
            File.Move(tempPath, Path, true);
            File.Delete(tempPath);
        }
    }
    public async Task WriteAsync(CancellationToken cancellationToken)
    {
       foreach (var line in GetLines(serializer => new EagerEnumerable<string>(serializer.SerializeAsync())))
            await writer.WriteLineAsync(line);

        if (Path is not null)
        {
            if (cancellationToken.IsCancellationRequested)
                File.Delete(tempPath);
            else
                File.Move(tempPath, Path, true);
        }
    }

    private IEnumerable<string> GetLines(Func<Serializer<string>, IEnumerable<string>> getSerializerLines) => File.Exists(Path)
        ? File.ReadLines(Path)
        .ReplaceSections(AddRemoveReplacements(serializers.Select(serializer => new SectionReplacement<string>(Wrap(serializer.Header, getSerializerLines(serializer)), line => line == serializer.Header, EndReplace, true))))
        : serializers.SelectMany(serializer => Wrap(serializer.Header, serializer.Serialize()));

    protected abstract bool EndReplace(string line);

    public void Dispose()
    {
        if (ownedWriter)
            Writer.Dispose();
    }
}
