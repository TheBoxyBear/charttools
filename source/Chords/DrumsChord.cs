using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;

using System;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by drums
    /// </summary>
    public class DrumsChord : LaneChord<DrumsNote, DrumsLane, DrumsChordModifiers>
    {
        protected override bool OpenExclusivity => false;

        internal override DrumsChordModifiers DefaultModifiers => DrumsChordModifiers.None;
        internal override bool ChartSupportedModifiers => true;

        public DrumsChord() : base() { }

        /// <inheritdoc cref="LaneChord(uint)"/>
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

        protected override IEnumerable<INote> GetNotes() => Notes;

        internal override IEnumerable<TrackObjectEntry> GetChartNoteData()
        {
            foreach (var note in Notes)
            {
                yield return ChartFormatting.NoteEntry(Position, note.Lane == DrumsLane.DoubleKick ? (byte)32 : note.Index, note.Length);

                if (note.IsCymbal)
                    yield return ChartFormatting.NoteEntry(Position, (byte)(note.Lane + 64), 0);

        internal override IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, WritingSession session)
        {
            if (Modifiers.HasFlag(DrumsChordModifiers.Flam))
                yield return ChartFormatting.NoteEntry(Position, 109, 0);
        }
    }
}
