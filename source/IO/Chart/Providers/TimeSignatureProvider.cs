using ChartTools.IO.Chart.Sessions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class TimeSignatureProvider : ITrackObjectProvider<TimeSignature>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<TimeSignature> source, WritingSession session)
        {
            HashSet<uint> ignored = new();

            foreach (var signature in source.Where(ts => session.DuplicateTrackObjectProcedure(ts.Position, ignored, "time signature")))
            {
                byte writtenDenominator = (byte)Math.Log2(signature.Denominator);
                string data = $"TS {signature.Numerator}";

                if (writtenDenominator == 1)
                    data += ' ' + writtenDenominator.ToString();

                yield return new(signature.Position, data);
            }
        }
    }
}
