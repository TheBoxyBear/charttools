﻿using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a standard five-fret instrument
    /// </summary>
    public class StandardChord : LaneChord<Note<StandardLane>, StandardLane, StandardChordModifier>
    {
        protected override bool OpenExclusivity => true;
        internal override bool ChartSupportedMoridier => !Modifiers.HasFlag(StandardChordModifier.ExplicitHopo);

        protected override StandardChordModifier DefaultModifiers => StandardChordModifier.None;

        public StandardChord() : base() { }
        /// <inheritdoc cref="Chord(uint)"/>
        public StandardChord(uint position) : base(position) { }
        /// <inheritdoc cref="LaneChord{TNote, TLane, TModifier}(uint)"/>
        /// <param name="notes">Notes to add</param>
        public StandardChord(uint position, params Note<StandardLane>[] notes) : this(position)
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
                Notes.Add(new Note<StandardLane>(note));
        }

        internal override IEnumerable<TrackObjectEntry> GetChartNoteData() => Notes.Select(note => ChartFormatting.NoteEntry(Position, note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.Length));

        internal override IEnumerable<TrackObjectEntry> GetChartModifierData(Chord? previous, WritingSession session)
        {
            bool isInvert = Modifiers.HasFlag(StandardChordModifier.HopoInvert);

            if (Modifiers.HasFlag(StandardChordModifier.ExplicitHopo) && (previous is null || previous.Position <= session.Formatting!.TrueHopoFrequency) != isInvert || isInvert)
                yield return ChartFormatting.NoteEntry(Position, 5, 0);
            if (Modifiers.HasFlag(StandardChordModifier.Tap))
                yield return ChartFormatting.NoteEntry(Position, 6, 0);
        }
    }
}
