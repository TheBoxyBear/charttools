using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Chart.Providers
{
    internal interface IChordProvider<TChord> : ITrackObjectProvider<TChord> where TChord : Chord
    {
        protected IEnumerable<string> GetNoteData();
    }
}
