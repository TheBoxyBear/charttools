using ChartTools.IO.Chart.Entries;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Parsers
{
    internal class SyncTrackParser : ChartPartParser
    {
        private SyncTrack? preResult, result;
        private HashSet<uint> ignoredTempos = new(), ignoredAnchors = new(), ignoredSignatures = new();
        private Func<uint, HashSet<uint>, string, bool>? includeObject;
        private const string parseFloatExceptionMessage = "Cannot parse value \"{0}\" to float.";

        public override SyncTrack? Result => result;

        protected override void HandleLine(string line)
        {
            TrackObjectEntry entry;
            try { entry = new(line); }
            catch (Exception e) { throw ChartParser.GetLineException(line, e); }

            Tempo? marker;

            switch (entry.Type)
            {
                // Time signature
                case "TS":
                    if (!includeObject!(entry.Position, ignoredSignatures, "time signature"))
                        break;

                    string[] split = ChartParser.GetDataSplit(entry.Data);

                    byte denominator;

                    if (!byte.TryParse(split[0], out byte numerator))
                        throw new FormatException($"Cannot parse numerator \"{split[0]}\" to byte.");

                    // Denominator is only written if not equal to 4
                    if (split.Length < 2)
                        denominator = 4;
                    else
                    {
                        if (byte.TryParse(split[1], out denominator))
                            //Denominator is written as its second power
                            denominator = (byte)Math.Pow(2, denominator);
                        else
                            throw new FormatException($"Cannot parse denominator \"{split[1]}\" to byte.");
                    }

                    preResult!.TimeSignatures.Add(new(entry.Position, numerator, denominator));
                    break;
                // Tempo
                case "B":
                    if (!includeObject!(entry.Position, ignoredTempos, "tempo marker"))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    if (float.TryParse(entry.Data, out float value))
                        value /= 1000;
                    else
                        throw new FormatException(string.Format(parseFloatExceptionMessage, entry.Data));

                    // Find the marker matching the position in case it was already added through a mention of anchor
                    marker = preResult!.Tempo.Find(m => m.Position == entry.Position);

                    if (marker is null)
                        preResult.Tempo.Add(new(entry.Position, value));
                    else
                        marker.Value = value;
                    break;
                // Anchor
                case "A":
                    if (!includeObject!(entry.Position, ignoredAnchors, "tempo anchor"))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    if (float.TryParse(entry.Data, out float anchor))
                        anchor /= 1000;
                    else
                        throw new FormatException(string.Format(parseFloatExceptionMessage, entry.Data));

                    // Find the marker matching the position in case it was already added through a mention of value
                    marker = preResult!.Tempo.Find(m => m.Position == entry.Position);

                    if (marker is null)
                        preResult.Tempo.Add(new(entry.Position, 0) { Anchor = anchor });
                    else
                        marker.Anchor = anchor;

                    break;
            }
        }

        protected override void PrepareParse()
        {
            preResult = new();
            ignoredTempos = new();
            ignoredAnchors = new();
            ignoredSignatures = new();

            includeObject = session!.Configuration.DuplicateTrackObjectPolicy switch
            {
                DuplicateTrackObjectPolicy.IncludeAll => ChartParser.IncludeSyncTrackAllPolicy,
                DuplicateTrackObjectPolicy.IncludeFirst => ChartParser.IncludeSyncTrackFirstPolicy,
                DuplicateTrackObjectPolicy.ThrowException => ChartParser.IncludeSyncTrackExceptionPolicy
            };

        }
        protected override void FinaliseParse() => result = preResult;

        public override void ApplyResultToSong(Song song) => song.SyncTrack = result;
    }
}
