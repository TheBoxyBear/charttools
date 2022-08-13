namespace ChartTools
{
    public abstract class LongTrackObject : TrackObject, ILongTrackObject
    {
        public uint Length { get; set; }
        public uint EndPosition => Position + Length;

        protected LongTrackObject() : base() { }
        /// <inheritdoc cref="TrackObject(uint)"/>
        protected LongTrackObject(uint position) : base(position) { }
    }
}
