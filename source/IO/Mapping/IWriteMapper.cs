using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Mapping
{
    internal interface IWriteMapper<TSource, TDest> : IMapper<TSource, TDest, WritingSession> { }
}
