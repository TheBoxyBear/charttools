using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Mapping
{
    internal interface IReadMapper<TSource, TDest> : IMapper<TSource, TDest, ReadingSession> { }
}
