using ChartTools.IO.Chart.Sessions;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Providers
{
    internal interface ITrackObjectProvider<T> where T : TrackObject
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<T> source, WritingSession session);
    }

    internal record TrackObjectProviderEntry(uint Position, string Line);
}
