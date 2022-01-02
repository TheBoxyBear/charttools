using ChartTools.IO.Chart.Sessions;
using System.Collections.Generic;
using System.Linq;

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

        protected override IEnumerable<string> GetModifierData(GHLChord? previous, GHLChord chord, WritingSession session)
        {
            var isInvert = chord.Modifier.HasFlag(GHLChordModifier.HopoInvert);

            if (chord.Modifier.HasFlag(GHLChordModifier.ExplicitHopo) && (previous is null || previous.Position <= session.HopoThreshold) != isInvert || isInvert)
                yield return ChartParser.GetNoteData(5, 0);
            if (chord.Modifier.HasFlag(GHLChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
