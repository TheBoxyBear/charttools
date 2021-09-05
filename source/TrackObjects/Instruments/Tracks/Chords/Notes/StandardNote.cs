using System;

namespace ChartTools
{
    /// <summary>
    /// Note played by a standard five-fret instrument
    /// </summary>
    public class StandardNote : Note
    {
        /// <summary>
        /// Fret to press
        /// </summary>
        public StandardNotes Note => (StandardNotes)NoteIndex;

        /// <summary>
        /// Creates an instance of <see cref="StandardNote"/>.
        /// </summary>
        /// <param name="note">Value of <see cref="Note"/></param>
        public StandardNote(StandardNotes note) : base((byte)note)
        {
            if (!Enum.IsDefined(note))
                throw new ArgumentException($"Note value is not defined.", nameof(note));
        }
    }
}
