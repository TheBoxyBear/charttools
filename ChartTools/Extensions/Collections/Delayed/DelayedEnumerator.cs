using System.Collections;

namespace ChartTools.Extensions.Collections;

internal class DelayedEnumerator<T>(DelayedEnumerableSource<T> source) : IEnumerator<T>
{
	public T Current { get; private set; }
	object? IEnumerator.Current => Current;
	public bool AwaitingItems => source.AwaitingItems;

	private bool WaitForItems()
	{
		while (source.Buffer.IsEmpty)
			if (!AwaitingItems && source.Buffer.IsEmpty)
				return false;

		return true;
	}

	public bool MoveNext()
	{
		if (!WaitForItems())
			return false;

		source.Buffer.TryDequeue(out T? item);
		Current = item!;

		return true;
	}

	void IDisposable.Dispose() => GC.SuppressFinalize(this);
	void IEnumerator.Reset() => throw new InvalidOperationException();
}
