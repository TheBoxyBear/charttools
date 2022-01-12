using System;

namespace ChartTools
{
    public interface ITrackObject : IEquatable<ITrackObject>
    {
        /// <summary>
        /// Position in beats from the start of the <see cref="Song"/> multiplied by <see cref="Metadata.Resolution"/>
        /// </summary>
        public uint Position { get; set; }

        public bool Equals(ITrackObject? other) => other is not null && other.Position == Position;
    }
}
