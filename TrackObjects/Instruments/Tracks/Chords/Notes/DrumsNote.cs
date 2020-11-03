namespace ChartTools
{
    /// <summary>
    /// Note played by drums
    /// </summary>
    public class DrumsNote : Note
    {
        /// <summary>
        /// Pad to hit
        /// </summary>
        public DrumsNotes Note => (DrumsNotes)NoteIndex;

        private bool _isCymbal = false;
        /// <summary>
        /// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
        /// </summary>
        /// <remarks><see cref="DrumsNotes.Green5Lane"/> notes cannot be cymbal.</remarks>
        public bool IsCymbal
        {
            get => _isCymbal;
            set
            {
                if ((Note == DrumsNotes.Red || Note == DrumsNotes.Green5Lane) && value)
                    throw new System.Exception("Red and 5-lane green notes cannot be cymbal.");

                _isCymbal = value;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="DrumsNote"/>.
        /// </summary>
        internal DrumsNote() : base(0) { }
        /// <summary>
        /// Creates an instance of <see cref="DrumsNote"/>.
        /// </summary>
        public DrumsNote(DrumsNotes note) : base((byte)note) { }
    }
}
