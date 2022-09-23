using System;

namespace ChartTools
{
    /// <summary>
    /// Marker that alters the tempo
    /// </summary>
    public class Tempo : TrackObjectBase
    {
        /// <summary>
        /// New tempo
        /// </summary>
        public float Value { get; set; }
        /// <summary>
        /// Locks the tempo to a specific time in the song independent to the sync track.
        /// </summary>
        public TimeSpan? Anchor { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="Tempo"/>.
        /// </summary>
        public Tempo(uint position, float value) : base(position) => Value = value;
    }
}
