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
        private readonly List<Anchor> orderedAnchors = new();
        private readonly List<TimeSignature> orderedSignatures = new();

        public SyncTrackParser(ReadingSession session) : base(session, ChartFormatting.SyncTrackHeader) { }

        protected override void HandleItem(string line)
        {
            TrackObjectEntry entry = new(line);

            switch (entry.Type)
            {
                case "TS": // Time signature
                    if (CheckDuplicate(orderedSignatures, "time signature", out int newIndex))
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
                    if (CheckDuplicate(orderedTempos, "tempo marker", out newIndex))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    var value = ValueParser.ParseFloat(entry.Data, "value") / 1000;
                    var tempo = new Tempo(entry.Position, value);

                    result.Tempo.Add(tempo);
                    orderedTempos.Add(tempo);
                    break;
                case "A": // Anchor
                    if (CheckDuplicate(orderedAnchors, "tempo anchor", out newIndex))
                        break;

                    // Floats are written by rounding to the 3rd decimal and removing the decimal point
                    var anchor = TimeSpan.FromSeconds(ValueParser.ParseFloat(entry.Data, "anchor") / 1000);

                    orderedAnchors.Insert(newIndex, new(entry.Position, anchor));
                    break;
            }

            bool CheckDuplicate<T>(IList<T> existing, string objectType, out int newIndex) where T : IReadOnlyTrackObject
            {
                var index = 0;
                var result = !session.DuplicateTrackObjectProcedure(entry.Position, objectType, () =>
                {
                    index = existing.BinarySearchIndex<T, uint>(entry.Position, t => t.Position, out bool exactMatch);

                    return exactMatch;
                });

                newIndex = index;

                return result;
            }
        }

        protected override void FinaliseParse()
        {
            foreach (var anchor in orderedAnchors)
            {
                // Find the marker matching the position in case it was already added through a mention of value
                var markerIndex = orderedTempos.BinarySearchIndex(anchor.Position, t => t.Position, out bool markerFound);

                if (markerFound)
                {
                    orderedTempos[markerIndex].Anchor = anchor.Value;
                    orderedTempos.RemoveAt(markerIndex);
                }
                else if (session.TempolessAnchorProcedure(anchor))
                    result.Tempo.Add(new(anchor.Position, 0) { Anchor = anchor.Value });
            }
        }

        public override void ApplyToSong(Song song) => song.SyncTrack = Result;
    }
}
