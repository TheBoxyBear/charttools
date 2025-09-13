using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart.Providers;

internal class ChordProvider : ISerializerDataProvider<Chord, TrackObjectEntry, ChartWritingSession>
{
	public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<Chord> source, ChartWritingSession session)
	{
		List<uint> orderedPositions = [];
		Chord? previousChord = null;

		foreach (Chord chord in source)
		{
			if (session.HandleDuplicate(chord.Position, "chord", () =>
			{
				int index = orderedPositions.BinarySearchIndex(chord.Position, out bool exactMatch);

				if (!exactMatch)
					orderedPositions.Insert(index, chord.Position);

				return exactMatch;
			}))
				foreach (TrackObjectEntry entry in (chord.ChartSupportedModifiers
					? chord.GetChartModifierData(previousChord, session)
					: session.GetUnsupportedModifierChordEntries(previousChord, chord)).Concat(chord.GetChartNoteData()))
					yield return entry;

			previousChord = chord;
		}
	}
}
