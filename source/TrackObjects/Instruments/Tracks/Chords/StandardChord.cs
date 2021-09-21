using ChartTools.IO.Chart;
using System;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a standard five-fret instrument
    /// </summary>
    public class StandardChord : Chord<Note<StandardLane>, StandardLane>
    {
        /// <inheritdoc cref="StandardChordModifier"/>
        public StandardChordModifier Modifier { get; set; } = StandardChordModifier.Natural;
        protected override bool OpenExclusivity => true;

        /// <inheritdoc cref="Chord(uint)"/>
        public StandardChord(uint position) : base(position) { }
        /// <inheritdoc cref="StandardChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public StandardChord(uint position, params Note<StandardLane>[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (Note<StandardLane> note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="StandardChord(uint, StandardNote[])"/>
        public StandardChord(uint position, params StandardLane[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (StandardLane note in notes)
                Notes.Add(new Note<StandardLane>(note));
        }

        /// <inheritdoc/>
        internal override IEnumerable<string> GetChartData()
        {
            foreach (Note<StandardLane> note in Notes)
                yield return ChartParser.GetNoteData(note.Lane == StandardLane.Open ? (byte)7 : (byte)(note.Lane - 1), note.SustainLength);

            if (Modifier.HasFlag(StandardChordModifier.Invert))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(StandardChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
