using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;

namespace ChartTools;

/// <summary>
/// Set of markers that define the time signature and tempo
/// </summary>
public class SyncTrack : IEmptyVerifiable
{
    /// <inheritdoc cref="IEmptyVerifiable.IsEmpty"/>
    public bool IsEmpty => Tempo.Count == 0 && TimeSignatures.Count == 0;

    /// <summary>
    /// Tempo markers
    /// </summary>
    public TempoMap Tempo { get; } = new();
    /// <summary>
    /// Time signature markers
    /// </summary>
    public List<TimeSignature> TimeSignatures { get; } = new();

    /// <summary>
    /// Reads a <see cref="SyncTrack"/> from a file.
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
    public static SyncTrack FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read<SyncTrack>(path, (".chart", path => ChartFile.ReadSyncTrack(path, config?.Chart)));

    /// <summary>
    /// Reads a <see cref="SyncTrack"/> from a file asynchronously using multitasking.
    /// </summary>
    /// <param name="path"><inheritdoc cref="FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
    /// <param name="cancellationToken">Token to request cancellation</param>
    /// <param name="config"><inheritdoc cref="FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
    /// <returns></returns>
    public static async Task<SyncTrack> FromFileAsync(string path, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<SyncTrack>(path, (".chart", path => ChartFile.ReadSyncTrackAsync(path, config?.Chart, cancellationToken)));
    public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, (".chart", (path, track) => ChartFile.ReplaceSyncTrack(path, track, config?.Chart)));
    public async Task ToFileAsync(string path, WritingConfiguration? config = default, CancellationToken cancellationToken = default) => await ExtensionHandler.WriteAsync(path, this, (".chart", (path, track) => ChartFile.ReplaceSyncTrackAsync(path, track, config?.Chart, cancellationToken)));
}
