using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a Guitar Hero Live instrument
    /// </summary>
    public class GHLChord : Chord<Note<GHLFret>, GHLFret>
    {
        /// <inheritdoc cref="GHLChordModifier"/>
        public GHLChordModifier Modifier { get; set; } = GHLChordModifier.None;
        protected override bool OpenExclusivity => true;

        /// <inheritdoc cref="Chord(uint)"/>
        public GHLChord(uint position) : base(position) { }
        /// <inheritdoc cref="GHLChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public GHLChord(uint position, params Note<GHLFret>[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (Note<GHLFret> note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="GHLChord(uint, GHLNote[])"/>
        public GHLChord(uint position, params GHLFret[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (GHLFret note in notes)
                Notes.Add(new Note<GHLFret>(note));
        }

        /// <inheritdoc/>
        internal override System.Collections.Generic.IEnumerable<string> GetChartData()
        {
            foreach (Note<GHLFret> note in Notes)
                yield return ChartParser.GetNoteData(note.Fret switch
                {
                    GHLFret.Open => 7,
                    GHLFret.Black1 => 3,
                    GHLFret.Black2 => 4,
                    GHLFret.Black3 => 8,
                    GHLFret.White1 => 0,
                    GHLFret.White2 => 1,
                    GHLFret.White3 => 2,
                }, note.SustainLength);

            if (Modifier.HasFlag(GHLChordModifier.Forced))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(GHLChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
