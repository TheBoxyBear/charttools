using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote, TLaneEnum> : Chord where TNote : Note<TLaneEnum> where TLaneEnum : struct, System.Enum
    {
        /// <summary>
        /// Notes in the <see cref="Chord{TNote}"/>
        /// </summary>
        public override NoteCollection<TNote, TLaneEnum> Notes { get; }

        /// <inheritdoc cref="Chord(uint)" path="/param"/>
        protected Chord(uint position) : base(position) => Notes = new(OpenExclusivity);
    }
}
