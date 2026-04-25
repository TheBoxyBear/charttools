using ChartTools.Events;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Configuration;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;
using ChartTools.Lyrics;
using ChartTools.Meta;

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
	/// Reads of a <see cref="Song"/> from a file.
	/// </summary>
	/// <param name="path">Path of the file to write to</param>
	/// <param name="config">Optional write config</param>
	/// <param name="formatting">Expected formatting</param>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Formatting may be used in other file formats")]
	public static Song FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default)
		=> ExtensionHandler.Read(path,
			(".chart", path => ChartFile.ReadSong(path, config?.Chart)),
			(".ini", path => new Song { Metadata = IniFile.ReadMetadata(path) }));

	/// <summary>
	/// Reads of a <see cref="Song"/> from a file asynchronously.
	/// </summary>
	/// <param name="path">Path of the file to write to</param>
	/// <param name="config">Optional write config</param>
	/// <param name="formatting">Expected formatting</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <remarks>Uses multi-threading to parse song components.</remarks>

	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Formatting may be used in other file formats")]
	public static async Task<Song> FromFileAsync(string path, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.ReadAsync(path,
			(".chart", path => ChartFile.ReadSongAsync(path, config, cancellationToken)),
			(".ini", async path => new Song
			{
				Metadata = await IniFile.ReadMetadataAsync(path, null, cancellationToken).ConfigureAwait(false)
			}))
		.ConfigureAwait(false);

	/// <summary>
	/// Reads a <see cref="Song"/> from a directory.
	/// </summary>
	/// <param name="directory">Path of the directory to read from</param>
	/// <param name="config">Optional read config</param>
	public static Song FromDirectory(string directory, ReadingConfiguration? config = default)
	{
		(Song? song, Metadata? metadata) = DirectoryHandler.FromDirectory(directory,
			(path, formatting) => FromFile(path, config, formatting));

		song ??= new();
		song.MergeMetadata(metadata);

		return song;
	}

	/// <summary>
	/// Reads a <see cref="Song"/> from a directory asynchronously.
	/// </summary>
	/// <param name="directory">Path of the directory to read from</param>
	/// <param name="config">Optional read config</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <remarks>Uses multi-threading to parse song components.</remarks>
	public static async Task<Song> FromDirectoryAsync(
		string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		(Song? song, Metadata? metadata) = await DirectoryHandler.FromDirectoryAsync(directory, async
			(path, formatting) => await FromFileAsync(path, config?.Chart, formatting, cancellationToken)
			.ConfigureAwait(false), cancellationToken)
			.ConfigureAwait(false);

		song ??= new();
		song.MergeMetadata(metadata);

		return song;
	}

	private void MergeMetadata(Metadata? iniMetadata)
	{
		if (iniMetadata is null)
			return;

		if (Metadata is not null)
			iniMetadata.Merge(Metadata);

		Metadata = iniMetadata;
	}
	#endregion

	/// <summary>
	/// Writes the <see cref="Song"/> to a file.
	/// </summary>
	/// <param name="path">Path of the file to write to</param>
	/// <param name="config">Optional write config</param>
	/// <param name="formatting">Formatting to apply</param>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Formatting may be used in other file formats")]
	public void ToFile(string path, WritingConfiguration? config = default, FormattingRules? formatting = default)
		=> ExtensionHandler.Write(path, this,
			(".chart", (path, song) => ChartFile.WriteSong(path, song, config?.Chart)),
			(".ini", static (path, song) =>
			{
				if (song.Metadata is not null)
					IniFile.WriteMetadata(path, song.Metadata);
			}));

	/// <summary>
	/// Writes the <see cref="Song"/> to a file asynchronously.
	/// </summary>
	/// <param name="path">Path of the file to write to</param>
	/// <param name="config">Optional write config</param>
	/// <param name="formatting">Formatting to apply</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <remarks>Uses multi-threading to serialize song components.</remarks>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Formatting may be used in other file formats")]
	public async Task ToFileAsync(string path, WritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.WriteAsync(path, this,
			(".chart", (path, song) => ChartFile.WriteSongAsync(path, song, config?.Chart, cancellationToken)),
			(".ini", (path, song) => song.Metadata is null
				? Task.CompletedTask
				: IniFile.WriteMetadataAsync(path, song.Metadata, cancellationToken)))
			.ConfigureAwait(false);
}
