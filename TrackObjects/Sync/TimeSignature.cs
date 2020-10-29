namespace ChartTools
{
    /// <summary>
    /// Marker that alters the time signature
    /// </summary>
    public class TimeSignature : TrackObject
    {
        /// <summary>
        /// Value of a beat
        /// </summary>
        public byte Numerator { get; set; }
        /// <summary>
        /// Beats per measure
        /// </summary>
        public byte Denominator { get; set; }

        /// <summary>
        /// Creates an instnace of <see cref="TimeSignature"/>.
        /// </summary>
        public TimeSignature(uint position, byte numerator, byte denominator) : base(position)
        {
            Numerator = numerator;
            Denominator = denominator;
        }
    }
}
