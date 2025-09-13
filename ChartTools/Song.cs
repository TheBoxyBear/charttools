using ChartTools.Events;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;
using ChartTools.Lyrics;
using ChartTools.Tools;

namespace ChartTools;

/// <summary>
/// Song playable in Clone Hero
/// </summary>
public class Song
{
	/// <summary>
	/// Set of information about the song not unrelated to instruments, syncing or events
	/// </summary>
	public Metadata? Metadata { get; set; }

	/// <inheritdoc cref="FormattingRules"/>
	public FormattingRules Formatting { get; set; } = new();

	/// <inheritdoc cref="ChartTools.SyncTrack"/>
	public SyncTrack? SyncTrack { get; set; }

	/// <summary>
	/// List of events common to all instruments
	/// </summary>
	public List<GlobalEvent>? GlobalEvents { get; set; }

	/// <inheritdoc cref="InstrumentSet"/>
	public InstrumentSet Instruments { get; set; } = new();

	public Vocals? Vocals { get; set; }

	public ChartSection? UnknownChartSections { get; set; } = [];

	#region Reading
	/// <summary>
	/// Reads all elements of a <see cref="Song"/> from a file.
	/// </summary>
	/// <param name="path">Path of the file</param>
	/// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
	/// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
	public static Song FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default)
		=> ExtensionHandler.Read(path,
			(".chart", path => ChartFile.ReadSong(path, config?.Chart)),
			(".ini", path => new Song { Metadata = IniFile.ReadMetadata(path) }));

	/// <summary>
	/// Reads all elements of a <see cref="Song"/> from a file asynchronously using multitasking.
	/// </summary>
	/// <param name="path"><inheritdoc cref="FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
	/// <param name="config"><inheritdoc cref="FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
	/// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
	/// <param name="cancellationToken">Token to request cancellation</param>
	public static async Task<Song> FromFileAsync(string path, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.ReadAsync(path,
			(".chart", path => ChartFile.ReadSongAsync(path, config, cancellationToken)))
		.ConfigureAwait(false);

	public static Song FromDirectory(string directory, ReadingConfiguration? config = default)
	{
		(var song, var metadata) = DirectoryHandler.FromDirectory(directory,
			(path, formatting) => FromFile(path, config, formatting));
		song ??= new();

		PropertyMerger.Merge(song.Metadata, true, true, metadata);

		return song;
	}

	public static async Task<Song> FromDirectoryAsync(
		string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		(var song, var metadata) = await DirectoryHandler.FromDirectoryAsync(directory, async
			(path, formatting) => await FromFileAsync(path, config?.Chart, formatting, cancellationToken)
			.ConfigureAwait(false), cancellationToken)
			.ConfigureAwait(false);
		song ??= new();

		PropertyMerger.Merge(song.Metadata, true, true, metadata);

		return song;
	}
	#endregion

	/// <summary>
	/// Writes the <see cref="Song"/> to a file.
	/// </summary>
	public void ToFile(string path, WritingConfiguration? config = default)
		=> ExtensionHandler.Write(path, this,
			(".chart", (path, song) => ChartFile.WriteSong(path, song, config?.Chart)));
	public async Task ToFileAsync(string path, WritingConfiguration? config = default, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.WriteAsync(path, this,
			(".chart", (path, song) => ChartFile.WriteSongAsync(path, song, config?.Chart, cancellationToken)))
		.ConfigureAwait(false);
}
