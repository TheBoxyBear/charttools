using System;

namespace ChartTools.IO
{
    internal struct Anchor : IReadOnlyTrackObject
    {
        public uint Position { get; }
        public TimeSpan Value { get; }

        public Anchor(uint position, TimeSpan value)
        {
            Position = position;
            Value = value;
        }
    }
}
