using System;
using System.Runtime.CompilerServices;

namespace ChartTools
{
    public abstract class LaneChord<TNote, TLane, TModifier> : Chord<TNote, TLane, TModifier> where TNote : Note<TLane>, new() where TLane : struct, Enum where TModifier : struct, Enum
    {
        protected abstract bool OpenExclusivity { get; }
        public override LaneNoteCollection<TNote, TLane> Notes { get; }

        protected LaneChord() : base() => Notes = new(OpenExclusivity);
        /// <inheritdoc cref="Chord{TNote, Tlane, TModifier}(uint)"/>
        protected LaneChord(uint position) : base(position) => Notes = new(OpenExclusivity);

        public override Note<TLane> CreateNote(byte index, uint length = 0)
        {
            var note = new TNote()
            {
                Lane = Unsafe.As<byte, TLane>(ref index),
                Length = length
            };

            Notes.Add(note);
            return note;
        }
    }
}
