using ChartTools.Collections.Alternating;
using ChartTools.IO.Chart.Providers;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class TrackObjectSerializer<T> : ChartSerializer<T>
    {
        public TrackObjectSerializer(string header, T content) : base(header, content) { }

        protected abstract IEnumerable<TrackObjectProviderEntry>[] LaunchProviders();

        protected override void GenerateLines() => preResult!.AddRange(new OrderedAlternatingEnumerable<uint, TrackObjectProviderEntry>(entry => entry.Position, LaunchProviders()).Select(entry => entry.Line));
    }
}
