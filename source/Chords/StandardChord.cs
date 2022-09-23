using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a standard five-fret instrument
    /// </summary>
    public class StandardChord : LaneChord<LaneNote<StandardLane>, StandardLane, StandardChordModifiers>
    {
        protected override bool OpenExclusivity => true;

        protected override StandardChordModifiers DefaultModifiers => StandardChordModifiers.None;
        internal override bool ChartSupportedModifiers => !Modifiers.HasFlag(StandardChordModifiers.ExplicitHopo);

        public StandardChord() : base() { }
        /// <inheritdoc cref="Chord(uint)"/>
        public StandardChord(uint position) : base(position) { }
        /// <inheritdoc cref="LaneChord{TNote, TLane, TModifier}(uint)"/>
        /// <param name="notes">Notes to add</param>
        public StandardChord(uint position, params LaneNote<StandardLane>[] notes) : this(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (var note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="StandardChord(uint, Note{StandardLane}[])"/>
        public StandardChord(uint position, params StandardLane[] notes) : this(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (StandardLane note in notes)
                Notes.Add(new LaneNote<StandardLane>(note));
        }

        protected override IEnumerable<INote> GetNotes() => Notes;

        internal override IEnumerable<TrackObjectEntry> GetChartData(LaneChord? previous, bool modifiers, FormattingRules formatting)
        {
            foreach (var entry in Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.Sustain)))
                yield return entry;

            bool isInvert = Modifiers.HasFlag(StandardChordModifiers.HopoInvert);

            if (Modifiers.HasFlag(StandardChordModifiers.ExplicitHopo) && (previous is null || previous.Position <= formatting.TrueHopoFrequency) != isInvert || isInvert)
                yield return ChartFormatting.NoteEntry(Position, 5, 0);
            if (Modifiers.HasFlag(StandardChordModifiers.Tap))
                yield return ChartFormatting.NoteEntry(Position, 6, 0);
        }
    }
}
