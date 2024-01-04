using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class ChordProvider : ISerializerDataProvider<LaneChord, TrackObjectEntry, ChartWritingSession>
{
    public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<LaneChord> source, ChartWritingSession session)
    {
        List<uint> orderedPositions = new();
        LaneChord? previousChord = null;

        foreach (var chord in source)
        {
            if (session.HandleDuplicate(chord.Position, "chord", () =>
            {
                var index = orderedPositions.BinarySearchIndex(chord.Position, out bool exactMatch);

                if (!exactMatch)
                    orderedPositions.Insert(index, chord.Position);

                return exactMatch;
            }))
                foreach (var entry in (chord.ChartSupportedModifiers ? chord.GetChartModifierData(previousChord, session) : session.GetUnsupportedModifierChordEntries(previousChord, chord)).Concat(chord.GetChartNoteData()))
                    yield return entry;

            previousChord = chord;
        }
    }
}
