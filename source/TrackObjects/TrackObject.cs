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
        public uint Position { get; set; }
        /// <summary>
        /// Creates a new instance of <see cref="TrackObject"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="Position"/></param>
        public TrackObject(uint position) => Position = position;

        public virtual int CompareTo(TrackObject? other) => Position.CompareTo(other?.Position);

        public override bool Equals(object? obj) => Equals(obj as TrackObject);
        public virtual bool Equals(TrackObject? other) => other is not null && other.Position == Position;

        public static bool operator ==(TrackObject a, TrackObject b) => a.Equals(b);
        public static bool operator !=(TrackObject a, TrackObject b) => !a.Equals(b);
    }
}
