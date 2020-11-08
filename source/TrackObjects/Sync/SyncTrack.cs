using ChartTools.Collections;
using ChartTools.IO;
using ChartTools.IO.Chart;

namespace ChartTools
{
    /// <summary>
    /// Set of markers that define the time signature and tempo
    /// </summary>
    public class SyncTrack
    {
        /// <summary>
        /// Tempo markers
        /// </summary>
        public UniqueListComparable<Tempo> Tempo { get; set; } = new UniqueListComparable<Tempo>();
        /// <summary>
        /// Time signature markers
        /// </summary>
        public UniqueListComparable<TimeSignature> TimeSignatures { get; set; } = new UniqueListComparable<TimeSignature>();

        /// <inheritdoc cref="ChartParser.ReadSyncTrack(string)"/>
        public static SyncTrack FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadSyncTrack));
    }
}
