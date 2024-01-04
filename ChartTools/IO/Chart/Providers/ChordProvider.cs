using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Common;
using ChartTools.IO.Configuration;

namespace ChartTools.IO.Chart.Providers;

internal class ChordProvider : ISerializerDataProvider<LaneChord, TrackObjectEntry, ChartWritingSession>
{
    public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<LaneChord> source, ChartWritingSession session)
    {
        List<uint> orderedPositions = [];
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
            {
                var flags = chord.ChartSupportedModifiers
                    ? ChordDataFlags.Chord | ChordDataFlags.Modifiers
                    : ICommonWritingConfiguration.GetUnsupportedModifierChordFlags(chord.Position, session.Configuration.UnsupportedModifiersPolicy);

                if (flags.HasFlag(ChordDataFlags.Chord))
                    foreach (var entry in chord.GetChartNoteData())
                        yield return entry;

                if (flags.HasFlag(ChordDataFlags.Modifiers))
                    foreach (var entry in chord.GetChartModifierData(previousChord, session.Formatting))
                        yield return entry;
            }

            previousChord = chord;
        }
    }
}
