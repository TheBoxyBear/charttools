using System.Collections;

namespace ChartTools.Internal.Collections;

internal class EagerEnumerable<T>(Task<IEnumerable<T>> source) : IEnumerable<T>
{
	private IEnumerable<T>? m_items;

	public IEnumerator<T> GetEnumerator()
	{
		if (m_items is null)
		{
			source.Wait();
			m_items = source.Result;
		}

		return m_items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
}
