using ChartTools.Extensions.Collections;
using ChartTools.IO.Sources;

namespace ChartTools.IO;

internal abstract class FileReader<T>(ReadingDataSource source) : IDisposable
{
    public DataSource Source { get; } = source;

    public bool IsReading { get; protected set; }

    public abstract IEnumerable<FileParser<T>> Parsers { get; }

    public abstract void Read();
    public abstract Task ReadAsync(CancellationToken cancellationToken);

    protected void CheckBusy()
    {
        if (IsReading)
            throw new InvalidOperationException("Cannot start read operation while the reader is busy.");
    }

    public virtual void Dispose() => Source.Dispose();
}

internal abstract class FileReader<T, TParser>(ReadingDataSource source) : FileReader<T>(source) where TParser : FileParser<T>
{
    public record ParserContentGroup(TParser Parser, DelayedEnumerableSource<T> Source);

    public override IEnumerable<TParser> Parsers => parserGroups.Select(g => g.Parser);

    protected readonly List<ParserContentGroup> parserGroups = [];
    protected readonly List<Task> parseTasks = [];

    protected abstract TParser? GetParser(string header);

    public override void Read()
    {
        CheckBusy();
        IsReading = true;

        parserGroups.Clear();
        parseTasks.Clear();

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

    protected abstract void ReadBase(bool async, CancellationToken cancellationToken);

    public override void Dispose()
    {
        foreach (var group in parserGroups)
            group.Source.Dispose();

        foreach (var task in parseTasks)
            task.Dispose();
    }
}
