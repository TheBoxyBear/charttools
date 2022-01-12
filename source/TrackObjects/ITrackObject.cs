using System;

namespace ChartTools
{
    public interface ITrackObject : IEquatable<ITrackObject>
    {
        /// <summary>
        /// Tick number on the track.
        /// </summary>
        /// <remarks>A tick represents a subdivision of a beat. The number of subdivisions per beat is stored in <see cref="Metadata.Resolution"/>.</remarks>
        public uint Position { get; set; }

        bool IEquatable<ITrackObject>.Equals(ITrackObject? other) => other is not null && other.Position == Position;
    }
}
