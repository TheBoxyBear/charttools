using ChartTools.Collections.Alternating;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class TrackObjectGroupSerializer<T> : ChartSerializer<T>
    {
        public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

        protected abstract IEnumerable<TrackObjectProviderEntry>[] LaunchProviders();

        public override IEnumerable<string> Serialize() => new OrderedAlternatingEnumerable<uint, TrackObjectProviderEntry>(entry => entry.Position, LaunchProviders()).Select(entry => entry.Line);
    }
}
