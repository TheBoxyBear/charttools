using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsers
{
    internal class SyncTrackParser : ChartParser
    {
        public override SyncTrack? Result => GetResult(result);
        private readonly SyncTrack result = new();

        private readonly HashSet<uint> ignoredTempos = new(), ignoredAnchors = new(), ignoredSignatures = new();
        private const string parseFloatExceptionMessage = "Cannot parse value \"{0}\" to float.";

        public override SyncTrack Result => GetResult(result);
        private SyncTrack result = new();

        public SyncTrackParser(ReadingSession session) : base(session) { }

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry;
            try { entry = new(line); }
            catch (Exception e) { throw ChartExceptions.Line(line, e); }

            Tempo? marker;

            switch (entry.Type)
            {
                // Time signature
                case "TS":
                    if (!session!.DuplicateTrackObjectProcedure!(entry.Position, ignoredSignatures, "time signature"))
                        break;

                    string[] split = ChartFile.GetDataSplit(entry.Data);

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

                    result.TimeSignatures.Add(new(entry.Position, numerator, denominator));
                    break;
                // Tempo
                case "B":
                    if (!session.DuplicateTrackObjectProcedure!(entry.Position, ignoredTempos, "tempo marker"))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    if (float.TryParse(entry.Data, out float value))
                        value /= 1000;
                    else
                        throw new FormatException(string.Format(parseFloatExceptionMessage, entry.Data));

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
                    if (float.TryParse(entry.Data, out float anchor))
                        anchor /= 1000;
                    else
                        throw new FormatException(string.Format(parseFloatExceptionMessage, entry.Data));

                    // Find the marker matching the position in case it was already added through a mention of value
                    marker = result.Tempo.Find(m => m.Position == entry.Position);

                    if (marker is null)
                        result.Tempo.Add(new(entry.Position, 0) { Anchor = anchor });
                    else
                        marker.Anchor = anchor;

                    break;
            }
        }

        public override void ApplyResultToSong(Song song) => song.SyncTrack = result;
    }
}
