using System;

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
        public GHLNotes Note => (GHLNotes)NoteIndex;

        /// <summary>
        /// Creates an instance of <see cref="GHLNote"/>.
        /// </summary>
        internal GHLNote() : base(0) { }
        /// <summary>
        /// Creates an instance of <see cref="GHLNote"/>.
        /// </summary>
        /// <param name="note">Value of <see cref="Note"/></param>
        public GHLNote(GHLNotes note) : base((byte)note)
        {
            if (!Enum.IsDefined(note))
                throw new ArgumentException($"Note value is not defined.", nameof(note));
        }
    }
}
