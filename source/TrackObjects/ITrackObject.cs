using System;

namespace ChartTools
{
    public interface ITrackObject : IComparable<ITrackObject>
    {
        public uint Position { get; set; }
    }
}
