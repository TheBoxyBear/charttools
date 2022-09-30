using System;

namespace ChartTools
{
    /// <summary>
    /// Base class for notes
    /// </summary>
    public class LaneNote<TLane> : LaneNote where TLane : struct, Enum
    {
        public override byte Index => (byte)(object)Lane;
        public TLane Lane { get; init; }

        public LaneNote() { }
        public LaneNote(TLane lane, uint sustain = 0)
        {
            Lane = lane;
            Sustain = sustain;
        }
    }
}
