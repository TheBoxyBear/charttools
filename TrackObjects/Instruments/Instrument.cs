using ChartTools.IO;
using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Base class for instruments
    /// </summary>
    public abstract class Instrument
    {
        /// <summary>
        /// Estimated difficulty
        /// </summary>
        public byte? Difficulty { get; set; }

        /// <inheritdoc cref="ChartParser.ReadDrums(string)"/>
        public static Instrument<DrumsChord> FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadDrums));
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, GHLInstrument)"/>
        public static Instrument<GHLChord> FromFile(string path, GHLInstrument instrument) => ExtensionHandler.Read(path, (".chart", (p) => ChartParser.ReadInstrument(p, instrument)));
        /// <inheritdoc cref="ChartParser.ReadInstrument(string, StandardInstrument)"/>
        public static Instrument<StandardChord> FromFile(string path, StandardInstrument instrument) => ExtensionHandler.Read(path, (".chart", (p) => ChartParser.ReadInstrument(p, instrument)));
    }
}
