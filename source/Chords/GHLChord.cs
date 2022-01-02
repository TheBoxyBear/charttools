using System;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a Guitar Hero Live instrument
    /// </summary>
    public class GHLChord : LaneChord<Note<GHLLane>, GHLLane, GHLChordModifier>
    {
        protected override bool OpenExclusivity => true;
        internal override bool ChartSupportedMoridier => !Modifier.HasFlag(GHLChordModifier.ExplicitHopo);

        /// <inheritdoc cref="Chord(uint)"/>
        public GHLChord(uint position) : base(position) { }
        /// <inheritdoc cref="GHLChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public GHLChord(uint position, params Note<GHLLane>[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (Note<GHLLane> note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="GHLChord(uint, GHLNote[])"/>
        public GHLChord(uint position, params GHLLane[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (GHLLane note in notes)
                Notes.Add(new Note<GHLLane>(note));
        }
    }
}
