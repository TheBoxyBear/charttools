using System;

namespace ChartTools
{
    /// <summary>
    /// Object located on a track
    /// </summary>
    public class TrackObject : ITrackObject
    {
        /// <summary>
        /// Position in beats from the start of the <see cref="Song"/> multiplied by <see cref="Metadata.Resolution"/>
        /// </summary>
        public virtual uint Position { get; set; }
        /// <summary>
        /// Creates a new instance of <see cref="TrackObject"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="Position"/></param>
        public TrackObject(uint position) => Position = position;

        public virtual int CompareTo(ITrackObject? other) => Position.CompareTo(other?.Position);

        public override bool Equals(object? obj) => Equals(obj as ITrackObject);
        public virtual bool Equals(ITrackObject? other) => other is not null && other.Position == Position;

        public static bool operator ==(TrackObject a, TrackObject b) => a.Equals(b);
        public static bool operator !=(TrackObject a, TrackObject b) => !a.Equals(b);
    }
}
