using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Mapping
{
    internal interface IMapper<TSource, TDest, TSession> where TSession : Session
    {
        public TDest Map(TSource source, TSession session);
    }
}
