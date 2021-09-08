using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a standard five-fret instrument
    /// </summary>
    public class StandardChord : Chord<StandardNote, StandardNotes>
    {
        /// <inheritdoc cref="StandardChordModifier"/>
        public StandardChordModifier Modifier { get; set; } = StandardChordModifier.None;
        protected override bool OpenExclusivity => true;

        /// <inheritdoc cref="Chord(uint)"/>
        public StandardChord(uint position) : base(position) { }
        /// <inheritdoc cref="StandardChord(uint)"/>
        /// <param name="notes">Notes to add</param>
        public StandardChord(uint position, params StandardNote[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (StandardNote note in notes)
                Notes.Add(note);
        }
        /// <inheritdoc cref="StandardChord(uint, StandardNote[])"/>
        public StandardChord(uint position, params StandardNotes[] notes) : base(position)
        {
            if (notes is null)
                throw new ArgumentNullException(nameof(notes));

            foreach (StandardNotes note in notes)
                Notes.Add(new StandardNote(note));
        }

        /// <inheritdoc/>
        internal override IEnumerable<string> GetChartData()
        {
            foreach (StandardNote note in Notes)
                yield return ChartParser.GetNoteData(note.Fret == StandardNotes.Open ? (byte)7 : (byte)(note.Fret - 1), note.SustainLength);

            if (Modifier.HasFlag(StandardChordModifier.Forced))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(StandardChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
