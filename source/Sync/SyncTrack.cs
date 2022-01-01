using ChartTools.IO;
using ChartTools.IO.Chart;

using System.Collections.Generic;

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
        public List<Tempo> Tempo { get; } = new();
        /// <summary>
        /// Time signature markers
        /// </summary>
        public List<TimeSignature> TimeSignatures { get; } = new();

        /// <inheritdoc cref="ChartParser.ReadSyncTrack(string, ReadingConfiguration)"/>
        public static SyncTrack FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", ChartParser.ReadSyncTrack));
        /// <inheritdoc cref="ChartParser.ReplaceSyncTrack(string, SyncTrack, WritingConfiguration)"/>
        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartParser.ReplaceSyncTrack));
    }
}
