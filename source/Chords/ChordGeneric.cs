using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote, Tlane> : Chord where TNote : Note<Tlane>, new() where Tlane : struct
    {
        public override abstract IEnumerable<TNote> Notes { get; }

        /// <inheritdoc cref="Chord(uint)" path="/param"/>
        protected Chord(uint position) : base(position) { }
    }
}
