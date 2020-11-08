namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote> : Chord where TNote : Note
    {
        /// <summary>
        /// Notes in the <see cref="Chord{TNote}"/>
        /// </summary>
        public virtual NoteCollection<TNote> Notes { get; }

        /// <summary>
        /// Creates an instance of <see cref="Chord{TNote}"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        public Chord(uint position) : base(position) { }
    }
}
