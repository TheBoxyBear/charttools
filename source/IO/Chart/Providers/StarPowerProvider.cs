using ChartTools.IO.Chart.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart.Providers
{
    internal class StarPowerProvider : ITrackObjectProvider<StarPowerPhrase>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<StarPowerPhrase> source, WritingSession session)
        {
            throw new NotImplementedException();
        }
    }
}
