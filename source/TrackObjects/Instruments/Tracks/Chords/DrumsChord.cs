using ChartTools.IO.Chart;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by drums
    /// </summary>
    public class DrumsChord : Chord<DrumsNote, DrumsLane>
    {
        /// <inheritdoc cref="DrumsChordModifier"/>
        public DrumsChordModifier Modifier { get; set; } = DrumsChordModifier.None;
        protected override bool OpenExclusivity => false;

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

        /// <inheritdoc/>
        internal override IEnumerable<string> GetChartData()
        {
            foreach (DrumsNote note in Notes)
            {
                yield return ChartParser.GetNoteData(note.Lane == DrumsLane.DoubleKick ? (byte)32 : note.NoteIndex, note.SustainLength);

                if (note.IsCymbal)
                    yield return ChartParser.GetNoteData((byte)(note.Lane + 64), 0);
            }

            if (Modifier.HasFlag(DrumsChordModifier.Flam))
                yield return ChartParser.GetNoteData(109, 0);
        }
    }
}
