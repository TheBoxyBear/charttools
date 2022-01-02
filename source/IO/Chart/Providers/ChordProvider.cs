using ChartTools.IO.Chart.Sessions;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartTools.IO.Chart.Providers
{
    internal abstract class ChordProvider<TChord> : ITrackObjectProvider<TChord> where TChord : Chord
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<TChord> source, WritingSession session)
        {
            HashSet<byte> ignored = new();
            TChord? previousChord = null;

            foreach (var chord in source)
            {
                foreach (var line in chord.ChartSupportedMoridier ? chord.GetChartData(previousChord, session) : session.GetChordLines(previousChord, chord))
                    yield return new(chord.Position, line);
                foreach (string value in chord.GetChartNoteData())
                    yield return new(chord.Position, ChartFormatting.Line(chord.Position.ToString(), value));
                previousChord = chord;
            }
        }

        /// <summary>
        /// Gets the data to write in a chart file.
        /// </summary>
        /// <returns>Enumerable of strings containing the data to add to each line</returns>
        protected abstract IEnumerable<string> GetNoteData(TChord chord, WritingSession session);
        protected abstract IEnumerable<string> GetModifierData(TChord? previous, TChord chord, WritingSession session);
    }
}
