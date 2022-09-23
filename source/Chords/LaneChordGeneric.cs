using System;

namespace ChartTools
{
    public abstract class LaneChord<TNote, TLane, TModifiers> : LaneChord, IChord
        where TNote : LaneNote<TLane>, new()
        where TLane : struct, Enum
        where TModifiers : struct, Enum
    {
        protected abstract bool OpenExclusivity { get; }

        public new LaneNoteCollection<TNote, TLane> Notes { get; }

        public TModifiers Modifiers { get; set; }
        internal abstract TModifiers DefaultModifiers { get; }

        protected LaneChord() : base() => Notes = new(OpenExclusivity);
        protected LaneChord(uint position) : base(position) => Notes = new(OpenExclusivity);
    }
}
