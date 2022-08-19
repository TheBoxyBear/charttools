using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Mapping;
using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Mapping
{
    internal class ChordMapper : IWriteMapper<IEnumerable<Chord>, IEnumerable<TrackObjectEntry>>
    {
        public IEnumerable<TrackObjectEntry> Map(IEnumerable<Chord> source, WritingSession session)
        {
            List<uint> orderedPositions = new();
            Chord? previousChord = null;

            foreach (var chord in source)
            {
                if (session.DuplicateTrackObjectProcedure(chord.Position, "chord", () =>
                {
                    var index = orderedPositions.BinarySearchIndex(chord.Position, out bool exactMatch);

                    if (!exactMatch)
                        orderedPositions.Insert(index, chord.Position);

                    return exactMatch;
                }))
                    foreach (var entry in (chord.ChartSupportedMoridier ? chord.GetChartModifierData(previousChord, session) : session.GetChordEntries(previousChord, chord)).Concat(chord.GetChartNoteData()))
                        yield return entry;

                previousChord = chord;
            }
        }
    }
}
