namespace ChartTools
{
    /// <summary>
    /// Marker that alters the tempo
    /// </summary>
    public class Tempo : TrackObject
    {
        /// <summary>
        /// New tempo
        /// </summary>
        public float Value { get; set; }
        /// <summary>
        /// If not <see langword="null"/>, overrides <see cref="TrackObject.Position"/> with a time in seconds from the start of the song
        /// </summary>
        public float? Anchor { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="Tempo"/>.
        /// </summary>
        public Tempo(uint position, float value) : base(position) => Value = value;
    }
}
