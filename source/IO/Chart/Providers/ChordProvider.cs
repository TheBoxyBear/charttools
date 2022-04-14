using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class ChordProvider: ISerializerDataProvider<Chord, TrackObjectEntry>
    {
        public IEnumerable<TrackObjectEntry> ProvideFor(IEnumerable<Chord> source, WritingSession session)
        {
            HashSet<byte> ignored = new();
            Chord? previousChord = null;

            foreach (var chord in source)
            {
                foreach (var entry in (chord.ChartSupportedMoridier ? chord.GetChartModifierData(previousChord, session) : session.GetChordEntries(previousChord, chord)).Concat(chord.GetChartNoteData()))
                    yield return entry;

                previousChord = chord;
            }
        }
    }
}
