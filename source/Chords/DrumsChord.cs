using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Sessions;

using System;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by drums
    /// </summary>
    public class DrumsChord : LaneChord<DrumsNote, DrumsLane, DrumsChordModifier>
    {
        protected override bool OpenExclusivity => false;
        internal override bool ChartSupportedMoridier => true;

        /// <inheritdoc cref="Chord(uint)"/>
        public DrumsChord(uint position) : base(position) { }
        /// <inheritdoc cref="DrumsChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public DrumsChord(uint position, params DrumsNote[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (DrumsNote note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="DrumsChord(uint, DrumsNote[])"/>
        public DrumsChord(uint position, params DrumsLane[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (DrumsLane note in notes)
                Notes.Add(new DrumsNote(note));
        }

        internal override IEnumerable<string> GetChartNoteData()
        {
            foreach (DrumsNote note in Notes)
            {
                yield return ChartFormatting.NoteData(note.Lane == DrumsLane.DoubleKick ? (byte)32 : note.NoteIndex, note.Length);

                if (note.IsCymbal)
                    yield return ChartFormatting.NoteData((byte)(note.Lane + 64), 0);
            }
        }

        internal override IEnumerable<string> GetChartModifierData(Chord? previous, WritingSession session)
        {
            if (Modifier.HasFlag(DrumsChordModifier.Flam))
                yield return ChartFormatting.NoteData(109, 0);
        }
    }
}
