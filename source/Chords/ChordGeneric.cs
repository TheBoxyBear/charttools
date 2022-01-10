using System;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Set of notes played simultaneously
    /// </summary>
    public abstract class Chord<TNote, Tlane, TModifier> : Chord where TNote : Note<Tlane>, new() where Tlane : struct where TModifier : Enum
    {
        public override abstract IEnumerable<TNote> Notes { get; }
        protected abstract TModifier DefaultModifier { get; }
        public TModifier Modifier { get; set; }
        /// <inheritdoc cref="Chord(uint)" path="/param"/>
        protected Chord(uint position) : base(position) => Modifier = DefaultModifier;
    }
}
