using System;

namespace ChartTools
{
    /// <summary>
    /// Object located on a track
    /// </summary>
    public class TrackObject : IComparable<TrackObject>, IEquatable<TrackObject>
    {
        /// <summary>
        /// Position in beats from the start of the <see cref="Song"/> multiplied by <see cref="Metadata.Resolution"/>
        /// </summary>
        public uint Position { get; }
        /// <summary>
        /// Creates a new instnace of <see cref="TrackObject"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="Position"/></param>
        public TrackObject(uint position) => Position = position;
        /// <inheritdoc/>
        /// <param name="other">Track object to compare to</param>
        public virtual int CompareTo(TrackObject other) => Position.CompareTo(other.Position);

        public bool Equals(TrackObject other) => Position == other?.Position;
    }
}
