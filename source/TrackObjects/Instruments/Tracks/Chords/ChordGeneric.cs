namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote, TNoteEnum> : Chord where TNote : Note where TNoteEnum : struct, System.Enum
    {
        /// <summary>
        /// Notes in the <see cref="Chord{TNote}"/>
        /// </summary>
        public NoteCollection<TNote, TNoteEnum> Notes { get; init; }

        /// <summary>
        /// Creates an instance of <see cref="Chord{TNote}"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        public Chord(uint position) : base(position) { }
    }
}
