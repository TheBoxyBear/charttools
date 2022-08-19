using ChartTools.IO.Configuration.Sessions;
using System.Collections.Generic;

namespace ChartTools.IO.Mapping
{
    internal interface IMapper<TSource, TDest, TSession> where TSession : Session
    {
        public IEnumerable<TDest> Map(TSource source, TSession session);
    }
}
