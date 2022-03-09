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
        /// <inheritdoc cref="IEmpty.IsEmpty"/>
        public bool IsEmpty => Tempo.Count == 0 && TimeSignatures.Count == 0;

        /// <summary>
        /// Tempo markers
        /// </summary>
        public List<Tempo> Tempo { get; } = new();
        /// <summary>
        /// Time signature markers
        /// </summary>
        public List<TimeSignature> TimeSignatures { get; } = new();

        /// <summary>
        /// Reads a <see cref="SyncTrack"/> from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static SyncTrack FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read<SyncTrack>(path, config, (".chart", ChartFile.ReadSyncTrack));
        /// <summary>
        /// Reads a <see cref="SyncTrack"/> from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        /// <param name="config"><inheritdoc cref="FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        /// <returns></returns>
        public static async Task<SyncTrack> FromFileAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<SyncTrack>(path, cancellationToken, config, (".chart", ChartFile.ReadSyncTrackAsync));
        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartFile.ReplaceSyncTrack));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync(path, this, cancellationToken, config, (".chart", ChartFile.ReplaceSyncTrackAsync));
    }
}
