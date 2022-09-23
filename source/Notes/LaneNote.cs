using System;

namespace ChartTools
{
    /// <summary>
    /// Base class for notes
    /// </summary>
    public class LaneNote<TLane> : INote where TLane : struct, Enum
    {
        public byte Index => (byte)(object)Lane;
        public TLane Lane { get; init; }
        /// <summary>
        /// Maximum length the note can be held for extra points (sustain)
        /// </summary>
        public uint Length { get; set; } = 0;

        public LaneNote() { }
        public LaneNote(TLane lane, uint length = 0)
        {
            Lane = lane;
            Length = length;
        }
    }
}
