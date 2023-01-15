using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.Extensions.Linq;

namespace ChartTools.IO.Chart.Providers;

internal class ChordProvider: ISerializerDataProvider<LaneChord, TrackObjectEntry>
{
    public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<LaneChord> source, WritingSession session)
    {
        List<uint> orderedPositions = new();
        LaneChord? previousChord = null;

        foreach (var chord in source)
        {
            if (session.DuplicateTrackObjectProcedure(chord.Position, "chord", () =>
            {
                var index = orderedPositions.BinarySearchIndex(chord.Position, out bool exactMatch);

                if (!exactMatch)
                    orderedPositions.Insert(index, chord.Position);

                return exactMatch;
            }))
                foreach (var entry in (chord.ChartSupportedModifiers ? chord.GetChartModifierData(previousChord, session) : session.GetChordEntries(previousChord, chord)).Concat(chord.GetChartNoteData()))
                    yield return entry;

            previousChord = chord;
        }
    }
}
