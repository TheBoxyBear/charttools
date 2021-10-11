﻿using ChartTools.IO.Chart;
using System;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a standard five-fret instrument
    /// </summary>
    public class StandardChord : LaneChord<Note<StandardLane>, StandardLane>
    {
        /// <inheritdoc cref="DrumsChordModifier"/>
        public StandardChordModifier Modifier { get; set; }
        public override byte ModifierKey
        {
            get => (byte)Modifier;
            set => Modifier = (StandardChordModifier)value;
        }

        protected override bool OpenExclusivity => true;
        public override LaneNoteCollection<Note<StandardLane>, StandardLane> Notes { get; }

        /// <inheritdoc cref="Chord(uint)"/>
        public StandardChord(uint position) : base(position) => Notes = new(OpenExclusivity);
        /// <inheritdoc cref="StandardChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public StandardChord(uint position, params Note<StandardLane>[] notes) : this(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (Note<StandardLane> note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="StandardChord(uint, StandardNote[])"/>
        public StandardChord(uint position, params StandardLane[] notes) : this(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (StandardLane note in notes)
                Notes.Add(new Note<StandardLane>(note));
        }

        /// <inheritdoc/>
        internal override IEnumerable<string> GetChartData(ChartParser.WritingSession session, ICollection<byte> ignored)
        {
            foreach (Note<StandardLane> note in Notes)
                yield return ChartParser.GetNoteData(note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.SustainLength);

            if (Modifier.HasFlag(StandardChordModifier.Invert))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(StandardChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }

        internal override bool ChartModifierSupported() => Modifier is StandardChordModifier.Natural or StandardChordModifier.Invert or StandardChordModifier.Tap;
    }
}
