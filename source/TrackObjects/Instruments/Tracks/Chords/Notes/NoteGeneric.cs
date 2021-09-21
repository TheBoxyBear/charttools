using System;

namespace ChartTools
{
    public class Note<TLaneEnum> : Note where TLaneEnum : struct, Enum
    {
        public TLaneEnum Lane => (TLaneEnum)(object)NoteIndex;

        public Note(TLaneEnum note) : base(Convert.ToByte(note))
        {
            if (!Enum.IsDefined(note))
                throw new ArgumentException($"Note value is not defined.", nameof(note));
        }
    }
}
