using System;

namespace ChartTools
{
    public abstract class LaneChord<TNote, TLane, TModifiers> : LaneChord, IChord
        where TNote : LaneNote<TLane>, new()
        where TLane : struct, Enum
        where TModifiers : struct, Enum
    {
        /// <summary>
        /// Defines if open notes can be mixed with other notes for this chord type. <see langword="true"/> indicated open notes cannot be mixed.
        /// </summary>
        protected abstract bool OpenExclusivity { get; }

        /// <summary>
        /// Notes in the chord
        /// </summary>
        public new LaneNoteCollection<TNote, TLane> Notes { get; }

        public TModifiers Modifiers { get; set; }
        internal abstract TModifiers DefaultModifiers { get; }

        public LaneChord() : base() => Notes = new(OpenExclusivity);
        public LaneChord(uint position) : base(position) => Notes = new(OpenExclusivity);

        public override LaneNote CreateNote(byte index) => new TNote() { Lane = (TLane)(object)index };
    }
}
