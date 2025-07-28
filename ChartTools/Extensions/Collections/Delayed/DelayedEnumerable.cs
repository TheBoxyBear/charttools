using System.Collections;

namespace ChartTools.Extensions.Collections;

public class DelayedEnumerable<T> : IEnumerable<T>
{
	private readonly DelayedEnumerator<T> m_enumerator;
	private readonly DelayedEnumerableSource<T> m_source;

	/// <summary>
	/// <see langword="true"/> if there are more items to be received
	/// </summary>
	public bool AwaitingItems => m_source.AwaitingItems;

	internal DelayedEnumerable(DelayedEnumerableSource<T> source)
	{
		m_source = source;
		m_enumerator = new(source);
	}

	public IEnumerable<T> EnumerateSynchronously()
	{
		while (AwaitingItems);
		return m_source.Buffer;
	}

	public IEnumerator<T> GetEnumerator() => m_enumerator;

	IEnumerator IEnumerable.GetEnumerator() => m_enumerator;
}
