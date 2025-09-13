using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

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
	public TempoMap Tempo { get; } = [];

	/// <summary>
	/// Time signature markers
	/// </summary>
	public List<TimeSignature> TimeSignatures { get; } = [];

	/// <summary>
	/// Reads a <see cref="SyncTrack"/> from a file.
	/// </summary>
	/// <param name="path">Path of the file to read from</param>
	/// <param name="config">Optional read config</param>
	/// <param name="formatting">Expected formatting</param>
	public static SyncTrack FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default)
		=> ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadSyncTrack(path, config?.Chart)));

	/// <summary>
	/// Reads a <see cref="SyncTrack"/> from a file asynchronously.
	/// </summary>
	/// <param name="path">Path of the file to read from</param>
	/// <param name="config">Optional read config</param>
	/// <param name="formatting">Expected formatting</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task<SyncTrack> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.ReadAsync(path,
			(".chart", path => ChartFile.ReadSyncTrackAsync(path, config?.Chart, cancellationToken)));

	public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, (".chart", (path, track)
		=> ChartFile.ReplaceSyncTrack(path, track, config?.Chart)));

	public async Task ToFileAsync(string path, WritingConfiguration? config = default, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.WriteAsync(path, this,
			(".chart", (path, track) => ChartFile.ReplaceSyncTrackAsync(path, track, config?.Chart, cancellationToken)))
		.ConfigureAwait(false);
}
