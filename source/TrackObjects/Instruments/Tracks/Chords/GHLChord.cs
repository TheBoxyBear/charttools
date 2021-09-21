using ChartTools.IO.Chart;
using System;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a Guitar Hero Live instrument
    /// </summary>
    public class GHLChord : Chord<Note<GHLLane>, GHLLane>
    {
        /// <inheritdoc cref="GHLChordModifier"/>
        public GHLChordModifier Modifier { get; set; } = GHLChordModifier.Natural;
        protected override bool OpenExclusivity => true;

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

        /// <inheritdoc/>
        internal override System.Collections.Generic.IEnumerable<string> GetChartData()
        {
            foreach (Note<GHLLane> note in Notes)
                yield return ChartParser.GetNoteData(note.Lane switch
                {
                    GHLLane.Open => 7,
                    GHLLane.Black1 => 3,
                    GHLLane.Black2 => 4,
                    GHLLane.Black3 => 8,
                    GHLLane.White1 => 0,
                    GHLLane.White2 => 1,
                    GHLLane.White3 => 2,
                }, note.SustainLength);

            if (Modifier.HasFlag(GHLChordModifier.Invert))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(GHLChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
