using ChartTools.Extensions.Collections;

namespace ChartTools.IO;

internal abstract class FileReader<T>(string path) : IDisposable
{
    public string Path { get; } = path;
    public bool IsReading { get; protected set; }
    public abstract IEnumerable<FileParser<T>> Parsers { get; }

    public abstract void Read();
    public abstract Task ReadAsync(CancellationToken cancellationToken);

    protected void CheckBusy()
    {
        if (IsReading)
            throw new InvalidOperationException("Cannot start read operation while the reader is busy.");
    }

    public abstract void Dispose();
}

internal abstract class FileReader<T, TParser>(string path, Func<string, TParser?> parserGetter) : FileReader<T>(path) where TParser : FileParser<T>
{
    public record ParserContentGroup(TParser Parser, DelayedEnumerableSource<T> Source);

    public override IEnumerable<TParser> Parsers => parserGroups.Select(g => g.Parser);

    protected readonly List<ParserContentGroup> parserGroups = [];
    protected readonly List<Task> parseTasks = [];
    protected readonly Func<string, TParser?> parserGetter = parserGetter;

    public override void Read()
    {
        CheckBusy();
        IsReading = true;

        ReadBase(false, CancellationToken.None);

        foreach (var group in parserGroups)
            group.Parser.Parse(group.Source.Enumerable.EnumerateSynchronously());

        IsReading = false;
    }
    public override async Task ReadAsync(CancellationToken cancellationToken)
    {
        CheckBusy();
        IsReading = true;

        ReadBase(true, cancellationToken);
        await Task.WhenAll(parseTasks);

        IsReading = false;
    }

    protected abstract void ReadBase(bool read, CancellationToken cancellationToken);

    public void Reset()
    {
        parseTasks.Clear();
        parserGroups.Clear();
    }

    public override async void Dispose()
    {
        foreach (var group in parserGroups)
            group.Source.Dispose();

        foreach (var task in parseTasks)
        {
            await task;
            task.Dispose();
        }
    }
}
