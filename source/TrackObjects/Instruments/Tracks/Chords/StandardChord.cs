using ChartTools.IO.Chart;
using System;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously by a standard five-fret instrument
    /// </summary>
    public class StandardChord : Chord<StandardNote>
    {
        /// <inheritdoc/>
        public override NoteCollection<StandardNote> Notes { get; } = new NoteCollection<StandardNote>(true);
        /// <inheritdoc cref="StandardChordModifier"/>
        public StandardChordModifier Modifier { get; set; } = StandardChordModifier.None;

        /// <summary>
        /// Creates an instance of <see cref="StandardChord"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        public StandardChord(uint position) : base(position) { }
        public StandardChord(uint position, params StandardNote[] notes) : this(position)
        {
            if (notes is null)
                throw new ArgumentNullException("Note array is null");

            foreach (StandardNote note in notes)
                Notes.Add(note);
        }
        public StandardChord(uint position, params StandardNotes[] notes) : this(position)
        {
            if (notes is null)
                throw new ArgumentNullException("Note array is null");

            foreach (StandardNotes note in notes)
                Notes.Add(new StandardNote(note));
        }

        /// <inheritdoc>/>
        internal override System.Collections.Generic.IEnumerable<string> GetChartData()
        {
            foreach (StandardNote note in Notes)
                yield return ChartParser.GetNoteData(note.Note == StandardNotes.Open ? 7 : (byte)(note.Note - 1), note.SustainLength);

            if (Modifier.HasFlag(StandardChordModifier.Forced))
                yield return ChartParser.GetNoteData(5, 0);
            if (Modifier.HasFlag(StandardChordModifier.Tap))
                yield return ChartParser.GetNoteData(6, 0);
        }
    }
}
