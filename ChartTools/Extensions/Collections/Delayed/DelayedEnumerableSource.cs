using System.Collections.Concurrent;

namespace ChartTools.Extensions.Collections;

public class DelayedEnumerableSource<T> : IDisposable
{
	public ConcurrentQueue<T> Buffer { get; } = new();
	public DelayedEnumerable<T> Enumerable { get; }
	public bool AwaitingItems { get; private set; } = true;

	public DelayedEnumerableSource() => Enumerable = new(this);

	public void Add(T item) => Buffer.Enqueue(item);
	public void EndAwait() => AwaitingItems = false;

	public void Dispose()
	{
		AwaitingItems = false;
		GC.SuppressFinalize(this);
	}
}
