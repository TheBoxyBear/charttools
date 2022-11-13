using ChartTools.Collections;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Serializaiton;

namespace ChartTools.IO;

internal abstract class TextFileWriter
{
    public string Path { get; }
    protected virtual string? PreSerializerContent => null;
    protected virtual string? PostSerializerContent => null;

    private readonly List<Serializer<string>> serializers;
    private readonly string tempPath = System.IO.Path.GetTempFileName();
    private readonly IEnumerable<string>? removedHeaders;

    public TextFileWriter(string path, IEnumerable<string>? removedHeaders, params Serializer<string>[] serializers)
    {
        Path = path;
        this.serializers = serializers.ToList();
        this.removedHeaders = removedHeaders;
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
        using (var writer = new StreamWriter(tempPath))
            foreach (var line in GetLines(serializer => serializer.Serialize()))
                writer.WriteLine(line);

        File.Copy(tempPath, Path, true);
        File.Delete(tempPath);
    }
    public async Task WriteAsync(CancellationToken cancellationToken)
    {
        using (var writer = new StreamWriter(tempPath))
            foreach (var line in GetLines(serializer => new EagerEnumerable<string>(serializer.SerializeAsync())))
                await writer.WriteLineAsync(line);

        if (cancellationToken.IsCancellationRequested)
            File.Delete(tempPath);
        else
            File.Move(tempPath, Path, true);
    }

    private IEnumerable<string> GetLines(Func<Serializer<string>, IEnumerable<string>> getSerializerLines) => File.Exists(Path)
        ? File.ReadAllLines(Path)
        .ReplaceSections(AddRemoveReplacements(serializers.Select(serializer => new SectionReplacement<string>(Wrap(serializer.Header, getSerializerLines(serializer)), line => line == serializer.Header, EndReplace, true))))
        : serializers.SelectMany(serializer => Wrap(serializer.Header, serializer.Serialize()));

    protected abstract bool EndReplace(string line);
}
