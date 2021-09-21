using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote, TLaneEnum> : Chord where TNote : Note<TLaneEnum> where TLaneEnum : struct, System.Enum
    {
        protected abstract bool OpenExclusivity { get; }

        /// <summary>
        /// Notes in the <see cref="Chord{TNote}"/>
        /// </summary>
        public NoteCollection<TNote, TLaneEnum> Notes { get; }

        /// <inheritdoc cref="Chord(uint)" path="/param"/>
        protected Chord(uint position) : base(position) => Notes = new(OpenExclusivity);

        public override IEnumerable<Note> GetNotes() => Notes;
    }
}
