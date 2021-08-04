using ChartTools.Collections.Unique;
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
        public UniqueList<Tempo> Tempo { get; } = new UniqueList<Tempo>((t, other) => t.Equals(other));
        /// <summary>
        /// Time signature markers
        /// </summary>
        public UniqueList<TimeSignature> TimeSignatures { get; } = new UniqueList<TimeSignature>((t, other) => t.Equals(other));

        /// <inheritdoc cref="ChartParser.ReadSyncTrack(string)"/>
        public static SyncTrack FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadSyncTrack));
        /// <inheritdoc cref="ChartParser.ReplaceSyncTrack(string, SyncTrack)"/>
        public void ToFile(string path) => ExtensionHandler.Write(path, this, (".chart", ChartParser.ReplaceSyncTrack));
    }
}
