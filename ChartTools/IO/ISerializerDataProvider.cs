using ChartTools.IO.Configuration;

namespace ChartTools.IO;

internal interface ISerializerDataProvider<TSource, TResult, TSession> where TSession : Session
{
    public IEnumerable<TResult> ProvideFor(IEnumerable<TSource> source, TSession session);
}
