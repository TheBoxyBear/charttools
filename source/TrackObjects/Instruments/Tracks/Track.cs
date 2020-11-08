using ChartTools.IO;
using ChartTools.IO.Chart;
using System.Collections.Generic;

namespace ChartTools
{
    /// <summary>
    /// Base class for tracks
    /// </summary>
    public class Track
    {
        /// <summary>
        /// Events specific to the <see cref="Track"/>
        /// </summary>
        public List<LocalEvent> LocalEvents { get; set; } = new List<LocalEvent>();

        /// <inheritdoc cref="ChartParser.ReadDrumsTrack(string, Difficulty)"/>
        public static Track<DrumsChord> FromFile(string path, Difficulty difficulty) => ExtensionHandler.Read(path, (".chart", (p) => ChartParser.ReadDrumsTrack(p, difficulty)));
        /// <inheritdoc cref="ChartParser.ReadTrack(string, GHLInstrument, Difficulty)"/>
        public static Track<GHLChord> FromFile(string path, GHLInstrument instrument, Difficulty difficulty) => ExtensionHandler.Read(path, (".chart", (p) => ChartParser.ReadTrack(p, instrument, difficulty)));
        /// <inheritdoc cref="ChartParser.ReadTrack(string, StandardInstrument, Difficulty)"/>
        public static Track<StandardChord> FromFile(string path, StandardInstrument instrument, Difficulty difficulty) => ExtensionHandler.Read(path, (".chart", (p) => ChartParser.ReadTrack(p, instrument, difficulty)));
    }
}
