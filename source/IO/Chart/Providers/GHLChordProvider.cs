using ChartTools.IO.Chart.Sessions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChartTools.IO.Chart.Providers
{
    internal class GHLChordProvider : ChordProvider<GHLChord>
    {
        protected override IEnumerable<string> GetNoteData(GHLChord chord, WritingSession session) => chord.Notes.Select(note => ChartParser.GetNoteData(note.Lane switch
            {
            GHLLane.Open => 7,
                GHLLane.Black1 => 3,
                GHLLane.Black2 => 4,
                GHLLane.Black3 => 8,
                GHLLane.White1 => 0,
                GHLLane.White2 => 1,
                GHLLane.White3 => 2,
            }, note.Length));

        protected override IEnumerable<string> GetModifierData(GHLChord chord, WritingSession session)
        {
            throw new NotImplementedException();
        }
    }
}
