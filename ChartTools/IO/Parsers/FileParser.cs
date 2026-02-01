namespace ChartTools.IO;

internal abstract class FileParser<T>
{
	public bool ResultReady { get; private set; }

	public abstract object? Result { get; }

	public async Task StartAsyncParse(IEnumerable<T> items)
	{
		await Task.Run(() => ParseBase(items)).ConfigureAwait(false);

		try { FinalizeParse(); }
		catch (Exception e) { throw GetFinalizeException(e); }
	}

	public void Parse(IEnumerable<T> items)
	{
		ParseBase(items);

		try { FinalizeParse(); }
		catch (Exception e) { throw GetFinalizeException(e); }
	}

	private void ParseBase(IEnumerable<T> items)
	{
		foreach (T item in items)
			try { HandleItem(item); }
			catch (Exception e) { throw GetHandleException(in item, e); }
	}

	protected abstract void HandleItem(in T item);

	protected virtual void FinalizeParse()
		=> ResultReady = true;

	protected TResult GetResult<TResult>(TResult result)
		=> ResultReady ? result : throw new Exception("Result is not ready.");

	protected abstract Exception GetHandleException(in T item, Exception innerException);

	protected abstract Exception GetFinalizeException(Exception innerException);
}
