using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote, Tlane> : Chord where TNote : Note<Tlane> where Tlane : struct
    {
        /// <summary>
        /// Notes in the <see cref="Chord{TNote}"/>
        /// </summary>
        public override NoteCollection<TNote, Tlane> Notes { get; }

        /// <inheritdoc cref="Chord(uint)" path="/param"/>
        protected Chord(uint position) : base(position) => Notes = new(OpenExclusivity);
    }
}
