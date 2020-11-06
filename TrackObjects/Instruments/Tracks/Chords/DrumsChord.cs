using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by drums
    /// </summary>
    public class DrumsChord : Chord<DrumsNote>
    {
        /// <inheritdoc/>
        public override NoteCollection<DrumsNote> Notes { get; } = new NoteCollection<DrumsNote>(false);
        /// <inheritdoc cref="DrumsChordModifier"/>
        public DrumsChordModifier Modifier { get; set; } = DrumsChordModifier.None;

        /// <summary>
        /// Creates an instance of <see cref="DrumsChord"/>
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        public DrumsChord(uint position) : base(position) { }

        /// <inheritdoc/>
        internal override System.Collections.Generic.IEnumerable<string> GetChartData()
        {
            foreach (DrumsNote note in Notes)
            {
                yield return ChartParser.GetNoteData((byte)note.Note, note.SustainLength);

                if (note.IsCymbal)
                    yield return ChartParser.GetNoteData((byte)(note.Note + 64), 0);
            }

            if (Modifier != DrumsChordModifier.None)
            {
                //Add once accent and ghost are added to Clone Hero
            }
        }
    }
}
