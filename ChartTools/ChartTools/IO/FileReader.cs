using ChartTools.Extensions.Collections;
using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;

namespace ChartTools.IO;

internal abstract class FileReader<T>(ReadingDataSource source) : IDisposable
{
	public DataSource Source { get; } = source;

	public bool IsReading { get; protected set; }

#if NET5_0_OR_GREATER
	public abstract IEnumerable<FileParser<T>> Parsers { get; }
#else
	public IEnumerable<FileParser<T>> Parsers
		=> GetFileParsers();

	protected abstract IEnumerable<FileParser<T>> GetFileParsers();
#endif

	public abstract void Read();

	public abstract Task ReadAsync(CancellationToken cancellationToken);

	protected void CheckBusy()
	{
		if (IsReading)
			throw new InvalidOperationException("Cannot start read operation while the reader is busy.");
	}

	public virtual void Dispose()
		=> Source.Dispose();
}

internal abstract class FileReader<T, TParser>(ReadingDataSource source) : FileReader<T>(source)
    where TParser : FileParser<T>
{
	public record ParserContentGroup(TParser Parser, DelayedEnumerableSource<T> Source);

#if NET5_0_OR_GREATER
	public override IEnumerable<TParser> Parsers
		=> m_parserGroups.Select(static g => g.Parser);
#else
	public new IEnumerable<TParser> Parsers
		=> m_parserGroups.Select(static g => g.Parser);

	protected override IEnumerable<FileParser<T>> GetFileParsers()
		=> Parsers;
#endif

	protected readonly List<ParserContentGroup> m_parserGroups = [];

	protected readonly List<Task> m_parseTasks = [];

	public override void Read()
	{
		CheckBusy();
		IsReading = true;

		m_parserGroups.Clear();
		m_parseTasks.Clear();

		ReadBase(false, CancellationToken.None);

		foreach (ParserContentGroup group in m_parserGroups)
			group.Parser.Parse(group.Source.Enumerable.EnumerateSynchronously());

		IsReading = false;
	}

	public override async Task ReadAsync(CancellationToken cancellationToken)
	{
		CheckBusy();
		IsReading = true;

		ReadBase(true, cancellationToken);
		await Task.WhenAll(m_parseTasks).ConfigureAwait(false);

		IsReading = false;
	}

	protected abstract void ReadBase(bool async, in CancellationToken cancellationToken);

	public override void Dispose()
	{
		foreach (ParserContentGroup group in m_parserGroups)
			group.Source.Dispose();

		foreach (Task task in m_parseTasks)
			task.Dispose();
	}
}
