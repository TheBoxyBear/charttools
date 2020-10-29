using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a Guitar Hero Live instrument
    /// </summary>
    public class GHLChord : Chord<GHLNote>
    {
         /// <inheritdoc/>
        public override NoteCollection<GHLNote> Notes { get; } = new NoteCollection<GHLNote>(true);
        /// <inheritdoc cref="GHLChordModifier"/>
        public GHLChordModifier Modifier { get; set; } = GHLChordModifier.None;

        /// <summary>
        /// Creates an instance of <see cref="GHLChord"/>.
        /// </summary>
        public GHLChord(uint position) : base(position) { }

        /// <inheritdoc/>
        internal override System.Collections.Generic.IEnumerable<string> GetChartData()
        {
            foreach (GHLNote note in Notes)
                yield return ChartParser.GetNoteData(note.Note switch
                {
                    GHLNotes.Open => 7,
                    GHLNotes.Black1 => 3,
                    GHLNotes.Black2 => 4,
                    GHLNotes.Black3 => 8,
                    GHLNotes.White1 => 0,
                    GHLNotes.White2 => 1,
                    GHLNotes.White3 => 2
                }, note.SustainLength);

            if (Modifier.HasFlag(GHLChordModifier.Forced))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(GHLChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
