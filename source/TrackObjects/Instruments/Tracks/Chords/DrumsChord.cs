using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by drums
    /// </summary>
    public class DrumsChord : Chord<DrumsNote, DrumsFret>
    {
        /// <inheritdoc cref="DrumsChordModifier"/>
        public DrumsChordModifier Modifier { get; set; } = DrumsChordModifier.None;
        protected override bool OpenExclusivity => false;

        /// <inheritdoc cref="Chord(uint)"/>
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
        public DrumsChord(uint position, params DrumsFret[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (DrumsFret note in notes)
                Notes.Add(new DrumsNote(note));
        }

        /// <inheritdoc/>
        internal override IEnumerable<string> GetChartData()
        {
            foreach (DrumsNote note in Notes)
            {
                yield return ChartParser.GetNoteData(note.Fret == DrumsFret.DoubleKick ? (byte)32 : note.NoteIndex, note.SustainLength);

                if (note.IsCymbal)
                    yield return ChartParser.GetNoteData((byte)(note.Fret + 64), 0);
            }

            if (Modifier.HasFlag(DrumsChordModifier.Flam))
                yield return ChartParser.GetNoteData(109, 0);
        }
    }
}
