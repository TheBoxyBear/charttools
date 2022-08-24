using ChartTools.Extensions.Linq;
using ChartTools.IO.Serializaiton;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serialization
{
    internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, string, TrackObjectEntry>
    {
        public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

        protected override IEnumerable<string> CombineMapperResults(IEnumerable<TrackObjectEntry>[] results) => results.AlternateBy(entry => entry.Position).Select(entry => entry.ToString());
    }
}
