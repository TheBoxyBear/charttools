using ChartTools.IO.Chart.Sessions;
using System.Collections.Generic;

namespace ChartTools.IO.Chart.Providers
{
    internal class DrumsChordProvider : ChordProvider<DrumsChord>
    {
        protected override IEnumerable<string> GetNoteData(DrumsChord chord, WritingSession session)
        {
            foreach (DrumsNote note in chord.Notes)
            {
                yield return ChartParser.GetNoteData(note.Lane == DrumsLane.DoubleKick ? (byte)32 : note.NoteIndex, note.Length);

                if (note.IsCymbal)
                    yield return ChartParser.GetNoteData((byte)(note.Lane + 64), 0);
            }
        }
        protected override IEnumerable<string> GetModifierData(DrumsChord? previous, DrumsChord chord, WritingSession session)
        {
            if (Modifier.HasFlag(DrumsChordModifier.Flam))
                yield return ChartParser.GetNoteData(109, 0);
        }

    }
}
