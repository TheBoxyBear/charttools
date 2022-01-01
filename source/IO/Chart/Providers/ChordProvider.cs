using ChartTools.IO.Chart.Sessions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartTools.IO.Chart.Providers
{
    internal abstract class ChordProvider<TChord> : ITrackObjectProvider<TChord> where TChord : Chord
    {
        public IEnumerable<TrackObjectProviderEntry> ProvideFor(IEnumerable<TChord> source, WritingSession session) => from chord in source
                                                                                                                       from line in GetNoteData(chord, session!).Concat(GetNoteData(chord, session!))
                                                                                                                       select new TrackObjectProviderEntry(chord.Position, line);

        protected abstract IEnumerable<string> GetNoteData(TChord chord, WritingSession session);
        protected abstract IEnumerable<string> GetModifierData(TChord chord, WritingSession session);
    }
}
