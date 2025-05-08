using System.Collections;

namespace ChartTools.Internal.Collections;

internal class EagerEnumerable<T>(Task<IEnumerable<T>> source) : IEnumerable<T>
{
	private IEnumerable<T>? items;

	public IEnumerator<T> GetEnumerator()
	{
		if (items is null)
		{
			source.Wait();
			items = source.Result;
		}

		return items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
