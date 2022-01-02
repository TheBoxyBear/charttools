using ChartTools.IO.Chart.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Providers
{
    internal class StandardChordProvider : ChordProvider<StandardChord>
    {
        protected override IEnumerable<string> GetNoteData(StandardChord chord, WritingSession session) => chord.Notes.Select(note => ChartParser.GetNoteData(note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.Length));
        protected override IEnumerable<string> GetModifierData(StandardChord? previous, StandardChord chord, WritingSession session)
        {
            bool isInvert = chord.Modifier.HasFlag(StandardChordModifier.HopoInvert);

            if (chord.Modifier.HasFlag(StandardChordModifier.ExplicitHopo) && (previous is null || previous.Position <= session.HopoThreshold) != isInvert || isInvert)
                yield return ChartParser.GetNoteData(5, 0);
            if (chord.Modifier.HasFlag(StandardChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
