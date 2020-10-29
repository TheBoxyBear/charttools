namespace ChartTools
{
    /// <summary>
    /// Base class for notes
    /// </summary>
    public abstract class Note
    {
        /// <summary>
        /// Numerical value of the note as writen in a chart file
        /// </summary>
        internal byte NoteIndex { get; set; }
        /// <summary>
        /// Maximum length the note can be held for extra points
        /// </summary>
        public uint SustainLength { get; set; } = 0;

        /// <summary>
        /// Creates an instance of <see cref="Note"/>.
        /// </summary>
        protected Note(byte noteIndex) => NoteIndex = noteIndex;
    }
}
