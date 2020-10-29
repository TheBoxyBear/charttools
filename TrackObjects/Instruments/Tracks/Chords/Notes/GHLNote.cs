namespace ChartTools
{
    /// <summary>
    /// Note played by a Guiter Hero Live instrument
    /// </summary>
    public class GHLNote : Note
    {
        /// <summary>
        /// Fret to press
        /// </summary>
        public GHLNotes Note { get => (GHLNotes)NoteIndex; }

        /// <summary>
        /// Creates an instance of <see cref="GHLNote"/>.
        /// </summary>
        internal GHLNote() : base(0) { }
        /// <summary>
        /// Creates an instance of <see cref="GHLNote"/>.
        /// </summary>
        public GHLNote(GHLNotes note) : base((byte)note) { }
    }
}
