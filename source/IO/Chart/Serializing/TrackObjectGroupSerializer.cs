using ChartTools.Collections.Alternating;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializing
{
    internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, string, TrackObjectEntry>
    {
        public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

        protected override IEnumerable<string> CombineMapperResults(IEnumerable<TrackObjectEntry>[] results) => new OrderedAlternatingEnumerable<uint, TrackObjectEntry>(entry => entry.Position, results).Select(entry => entry.ToString());
    }
}
