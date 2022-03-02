using ChartTools.Collections.Alternating;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, string, TrackObjectProviderEntry>
    {
        public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

        protected override IEnumerable<string> CombineProviderResults(IEnumerable<TrackObjectProviderEntry>[] results) => new OrderedAlternatingEnumerable<uint, TrackObjectProviderEntry>(entry => entry.Position, results).Select(entry => entry.Line);
    }
}
