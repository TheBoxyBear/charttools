namespace ChartTools
{
    /// <summary>
    /// Object located on a track
    /// </summary>
    public class TrackObject : ITrackObject
    {
        /// <inheritdoc cref="ITrackObject.Position"/>
        public virtual uint Position { get; set; }

        protected TrackObject() { }
        /// <param name="position">Position of the object</param>
        protected TrackObject(uint position) => Position = position;

        public virtual int CompareTo(TrackObject? other) => Position.CompareTo(other?.Position);

        public override bool Equals(object? obj) => Equals(obj as ITrackObject);
        public bool Equals(ITrackObject? other) => ((ITrackObject)this).Equals(other);

        public static bool operator ==(TrackObject a, TrackObject b) => a.Equals(b);
        public static bool operator !=(TrackObject a, TrackObject b) => !a.Equals(b);

        public override int GetHashCode() => unchecked((int)Position);
    }
}
