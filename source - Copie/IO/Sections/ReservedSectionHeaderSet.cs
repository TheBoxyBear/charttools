using System.Collections;

namespace ChartTools.IO.Sections;

public class ReservedSectionHeaderSet : IEnumerable<ReservedSectionHeader>
{
    private readonly IEnumerable<ReservedSectionHeader> _headers;

    public ReservedSectionHeaderSet(IEnumerable<ReservedSectionHeader> headers) => _headers = headers;

    public IEnumerator<ReservedSectionHeader> GetEnumerator() => _headers.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
