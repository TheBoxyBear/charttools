using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

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

        protected override DrumsChordModifier DefaultModifiers => DrumsChordModifier.None;

        public DrumsChord() : base() { }

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

        internal override IEnumerable<TrackObjectEntry> GetChartNoteData()
        {
            foreach (DrumsNote note in Notes)
            {
                yield return ChartFormatting.NoteEntry(Position, note.Lane == DrumsLane.DoubleKick ? (byte)32 : note.NoteIndex, note.Length);

                if (note.IsCymbal)
                    yield return ChartFormatting.NoteEntry(Position, (byte)(note.Lane + 64), 0);
            }
        }

        internal override IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, WritingSession session)
        {
            if (Modifiers.HasFlag(DrumsChordModifier.Flam))
                yield return ChartFormatting.NoteEntry(Position, 109, 0);
        }
    }
}
