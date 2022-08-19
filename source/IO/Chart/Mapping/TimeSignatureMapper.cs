using ChartTools.IO.Chart.Entries;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Mapping
{
    internal class TimeSignatureMapper : UniqueTrackObjectMapper<TimeSignature>
    {
        protected override string ObjectType => "time signature";

        protected override IEnumerable<TrackObjectEntry> GetEntries(TimeSignature item)
        {
            byte writtenDenominator = (byte)Math.Log2(item.Denominator);
            string data = item.Numerator.ToString();

            if (writtenDenominator == 1)
                data += ' ' + writtenDenominator.ToString();

            yield return new(item.Position, "TS", data);
        }
    }
}
