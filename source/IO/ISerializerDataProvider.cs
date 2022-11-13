using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO;

internal interface ISerializerDataProvider<TSource, TResult>
{
    public IEnumerable<TResult> ProvideFor(IEnumerable<TSource> source, WritingSession session);
}
