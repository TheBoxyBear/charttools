
namespace ChartTools.Extensions.Collections.Unique;

public class UniqueSelfCollection<T> : UniqueCollection<T, T>
{
    public UniqueSelfCollection(IEnumerable<T>? items = null) : base(i => i, items) { }
}
