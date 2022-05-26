using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Parsing
{
    internal class SyncTrackParser : ChartParser
    {
        public override SyncTrack Result => GetResult(result);
        private readonly SyncTrack result = new();

        private readonly List<Tempo> orderedTempos = new();
        private readonly List<uint> orderedAnchorPositions = new();
        private readonly List<TimeSignature> orderedSignatures = new();

        public SyncTrackParser(ReadingSession session) : base(session, ChartFormatting.SyncTrackHeader) { }

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry = new(line);

            Tempo? marker;

            switch (entry.Type)
            {
                case "TS": // Time signature
                    if (CheckDuplicateTrackObject(orderedSignatures, "time signature", out int newIndex))
                        break;

                    string[] split = ChartFormatting.SplitData(entry.Data);

                    var numerator = ValueParser.ParseByte(split[0], "numerator");
                    byte denominator = 4;

                    // Denominator is only written if not equal to 4
                    if (split.Length >= 2)
                        denominator = (byte)Math.Pow(2, ValueParser.ParseByte(split[1], "denominator"));

                    var signature = new TimeSignature(entry.Position, numerator, denominator);

                    result.TimeSignatures.Add(signature);
                    orderedSignatures.Insert(newIndex, signature);
                    break;
                case "B": // Tempo
                    if (CheckDuplicateTrackObject(orderedTempos, "tempo marker", out newIndex))
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
                case "A": // Anchor
                    if (CheckDuplicatePosition(orderedAnchorPositions, "tempo anchor", out newIndex))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    var anchor = TimeSpan.FromSeconds(ValueParser.ParseFloat(entry.Data, "anchor") / 1000);

                    // Find the marker matching the position in case it was already added through a mention of value
                    var markerIndex = orderedTempos.BinarySearchIndex(entry.Position, t => t.Position, out bool exactMatch);
                    marker = orderedTempos[markerIndex];

                    if (marker is null)
                    {
                        var tempo = new Tempo(entry.Position, 0) { Anchor = anchor };

                        result.Tempo.Add(tempo);
                        orderedTempos.Insert(markerIndex, tempo);
                    }
                    else
                        marker.Anchor = anchor;

                    orderedAnchorPositions.Insert(newIndex, entry.Position);

                    break;
            }

            bool CheckDuplicatePosition(IList<uint> existing, string objectType, out int newIndex) => CheckDuplicate(existing, objectType, p => p, out newIndex);
            bool CheckDuplicateTrackObject<T>(IList<T> existing, string objecType, out int newIndex) where T : TrackObject => CheckDuplicate(existing, objecType, t => t.Position, out newIndex);
            bool CheckDuplicate<T>(IList<T> existing, string objectType, Func<T, uint> positionSelector, out int newIndex)
            {
                var index = 0;
                var result = !session.DuplicateTrackObjectProcedure(entry.Position, objectType, () =>
                {
                    index = existing.BinarySearchIndex<T, uint>(entry.Position, positionSelector, out bool exactMatch);

                    return exactMatch;
                });

                newIndex = index;

                return result;
            }
        }

        public override void ApplyToSong(Song song) => song.SyncTrack = Result;
    }
}
