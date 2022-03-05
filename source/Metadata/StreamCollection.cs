using ChartTools.IO.Ini;

namespace ChartTools
{
    /// <summary>
    /// Set of audio files to play and mute during gameplay
    /// </summary>
    /// <remarks>Instrument audio may be muted when chords of the respective instrument are missed</remarks>
    public class StreamCollection
    {
        /// <summary>
        /// Location of the base audio file
        /// </summary>
        public string? Music { get; set; }
        /// <summary>
        /// Location of the guitar audio file
        /// </summary>
        public string? Guitar { get; set; }
        /// <summary>
        /// Location of the bass audio
        /// </summary>
        public string? Bass { get; set; }
        /// <summary>
        /// Location of the rhythm guitar audio file
        /// </summary>
        public string? Rhythm { get; set; }
        /// <summary>
        /// Location of the keys audio file
        /// </summary>
        public string? Keys { get; set; }
        /// <summary>
        /// Location of the drums' kicks audio file
        /// </summary>
        /// <remarks>Can include all drums audio</remarks>
        public string? Drum { get; set; }
        /// <summary>
        /// Location of the drums' snares audio file
        /// </summary>
        /// <remarks>Can include all drums audio except kicks</remarks>
        public string? Drum2 { get; set; }
        /// <summary>
        /// Location of the drum's toms audio file
        /// </summary>
        /// <remarks>Can include toms and cymbals</remarks>
        public string? Drum3 { get; set; }
        /// <summary>
        /// Location of the drum's cymbals audio file
        /// </summary>
        public string? Drum4 { get; set; }
        /// <summary>
        /// Location of the vocals audio file
        /// </summary>
        public string? Vocal { get; set; }
        /// <summary>
        /// Location of the crowd reaction audio file
        /// </summary>
        public string? Crowd { get; set; }
    }
}
