using System;

namespace ChartTools
{
    /// <summary>
    /// Object located on a track
    /// </summary>
    public abstract class TrackObject : IComparable<TrackObject>
    {
        /// <summary>
        /// Position in beats from the start of the <see cref="Song"/> multiplied by <see cref="Metadata.Resolution"/>
        /// </summary>
        public uint Position { get; set; }
        /// <summary>
        /// Creates a new instnace of <see cref="TrackObject"/>.
        /// </summary>
        public TrackObject(uint position) => Position = position;
        /// <inheritdoc/>
        public virtual int CompareTo(TrackObject other) => Position.CompareTo(other.Position);
    }
}
