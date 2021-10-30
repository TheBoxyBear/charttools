using System;

namespace ChartTools
{
    public abstract class LaneChord<TNote, TLane, TModifier> : Chord<TNote, TLane, TModifier> where TNote : Note<TLane>, new() where TLane : struct, Enum where TModifier : struct, Enum
    {
        protected abstract bool OpenExclusivity { get; }
        public override LaneNoteCollection<TNote, TLane> Notes { get; }

        public LaneChord(uint position) : base(position) => Notes = new(OpenExclusivity);
    }
}
