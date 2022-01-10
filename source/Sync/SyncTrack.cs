using ChartTools.Internal;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools
{
    /// <summary>
    /// Set of markers that define the time signature and tempo
    /// </summary>
    public class SyncTrack : IEmpty
    {
        public bool IsEmpty => Tempo.Count == 0 && TimeSignatures.Count == 0;

        /// <summary>
        /// Tempo markers
        /// </summary>
        public List<Tempo> Tempo { get; } = new();
        /// <summary>
        /// Time signature markers
        /// </summary>
        public List<TimeSignature> TimeSignatures { get; } = new();

        public static SyncTrack FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read<SyncTrack>(path, config, (".chart", ChartReader.ReadSyncTrack));
        public static async Task<SyncTrack> FromFileAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config) => await ExtensionHandler.ReadAsync<SyncTrack>(path, cancellationToken, config, (".chart", ChartReader.ReadSyncTrackAsync));
        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartWriter.ReplaceSyncTrack));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync(path, this, cancellationToken, config, (".chart", ChartWriter.ReplaceSyncTrackAsync));
    }
}
