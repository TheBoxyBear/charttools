using ChartTools.IO.Configuration.Sessions;
using System.Collections.Generic;

namespace ChartTools.IO
{
    internal interface ISerializerDataProvider<TSource, TResult>
    {
        public IEnumerable<TResult> ProvideFor(IEnumerable<TSource> source, WritingSession session);
    }
}
