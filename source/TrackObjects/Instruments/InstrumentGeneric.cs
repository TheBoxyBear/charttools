namespace ChartTools
{
    /// <summary>
    /// Set of tracks common to an instrument
    /// </summary>
    public class Instrument<TChord> : Instrument where TChord : Chord
    {
        /// <summary>
        /// Easy track
        /// </summary>
        public Track<TChord> Easy { get; set; }
        /// <summary>
        /// Medium track
        /// </summary>
        public Track<TChord> Medium { get; set; }
        /// <summary>
        /// Hard track
        /// </summary>
        public Track<TChord> Hard { get; set; }
        /// <summary>
        /// Expert track
        /// </summary>
        public Track<TChord> Expert { get; set; }
    }
}
