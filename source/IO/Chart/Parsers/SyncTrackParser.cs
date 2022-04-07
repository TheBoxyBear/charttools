using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal class SyncTrackParser : ChartParser
    {
        public override SyncTrack Result => GetResult(result);
        private readonly SyncTrack result = new();

        private readonly HashSet<uint> ignoredTempos = new(), ignoredAnchors = new(), ignoredSignatures = new();

        public SyncTrackParser(ReadingSession session) : base(session) { }

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry = new(line);

            Tempo? marker;

            switch (entry.Type)
            {
                // Time signature
                case "TS":
                    if (!session!.DuplicateTrackObjectProcedure!(entry.Position, ignoredSignatures, "time signature"))
                        break;

                    string[] split = ChartFormatting.SplitData(entry.Data);

                    var numerator = ValueParser.ParseByte(split[0], "numerator");
                    byte denominator = 4;

                    // Denominator is only written if not equal to 4
                    if (split.Length >= 2)
                        denominator = (byte)Math.Pow(2, ValueParser.ParseByte(split[1], "denominator"));

                    result.TimeSignatures.Add(new(entry.Position, numerator, denominator));
                    break;
                // Tempo
                case "B":
                    if (!session.DuplicateTrackObjectProcedure!(entry.Position, ignoredTempos, "tempo marker"))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    var value = ValueParser.ParseFloat(entry.Data, "value") / 1000;

                    // Find the marker matching the position in case it was already added through a mention of anchor
                    marker = result.Tempo.Find(m => m.Position == entry.Position);

                    if (marker is null)
                        result.Tempo.Add(new(entry.Position, value));
                    else
                        marker.Value = value;
                    break;
                // Anchor
                case "A":
                    if (!session.DuplicateTrackObjectProcedure!(entry.Position, ignoredAnchors, "tempo anchor"))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    var anchor = ValueParser.ParseFloat(entry.Data, "anchor") / 1000;

                    // Find the marker matching the position in case it was already added through a mention of value
                    marker = result.Tempo.Find(m => m.Position == entry.Position);

                    if (marker is null)
                        result.Tempo.Add(new(entry.Position, 0) { Anchor = anchor });
                    else
                        marker.Anchor = anchor;

                    break;
            }
        }

        public override void ApplyToSong(Song song) => song.SyncTrack = Result;
    }
}
