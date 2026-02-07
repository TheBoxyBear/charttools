using System.Collections;

namespace ChartTools.IO.Sections;

public class ReservedSectionHeaderSet(IEnumerable<ReservedSectionHeader> headers)
	: IEnumerable<ReservedSectionHeader>
{
	public IEnumerator<ReservedSectionHeader> GetEnumerator()
		=> headers.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
