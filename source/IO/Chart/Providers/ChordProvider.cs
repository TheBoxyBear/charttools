using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;


namespace ChartTools.IO.Chart.Providers
{
    internal class ChordProvider: ITrackObjectProvider<Chord>
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<Chord> source, WritingSession session)
        {
            HashSet<byte> ignored = new();
            Chord? previousChord = null;

            foreach (var chord in source)
            {
                foreach (var line in chord.ChartSupportedMoridier ? chord.GetChartModifierData(previousChord, session) : session.GetChordLines(previousChord, chord))
                    yield return new(chord.Position, line);
                foreach (string value in chord.GetChartNoteData())
                    yield return new(chord.Position, ChartFormatting.Line(chord.Position.ToString(), value));
                previousChord = chord;
            }
        }
    }
}
