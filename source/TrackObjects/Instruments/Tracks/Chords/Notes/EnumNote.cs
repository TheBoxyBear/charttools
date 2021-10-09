using System;

namespace ChartTools
{
    public class EnumNote<TLane> : Note<TLane> where TLane : struct, Enum
    {
        public EnumNote(TLane lane) : base(lane)
        {
            if (!Enum.IsDefined(lane))
                throw new ArgumentException($"Note value is not defined.", nameof(lane));
        }
    }
}
