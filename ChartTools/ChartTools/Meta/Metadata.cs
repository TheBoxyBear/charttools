using ChartTools.Extensions;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Serializing;
using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;
using ChartTools.Meta.Mapping;

namespace ChartTools.Meta;

/// <summary>
/// Set of miscellaneous information about a <see cref="Song"/>
/// </summary>
public class Metadata
{
	#region Properties
	/// <summary>
	/// Title of the <see cref="Song"/>
	/// </summary>
	[ChartKeySerializable(ChartFormatting.Title)]
	[IniKeySerializable(IniFormatting.Title)]
	public string? Title { get; set; }

	/// <summary>
	/// Artist or band behind the <see cref="Song"/>
	/// </summary>
	[ChartKeySerializable(ChartFormatting.Artist)]
	[IniKeySerializable(IniFormatting.Artist)]
	public string? Artist { get; set; }

	/// <summary>
	/// Album featuring the <see cref="Song"/>
	/// </summary>
	[ChartKeySerializable(ChartFormatting.Album)]
	[IniKeySerializable(IniFormatting.Album)]
	public string? Album { get; set; }

	/// <summary>
	/// Track number of the song within the album
	/// </summary>
	public ushort? AlbumTrack { get; set; }

	/// <summary>
	/// Playlist that the song should show up in
	/// </summary>
	[IniKeySerializable(IniFormatting.Playlist)]
	public string? Playlist { get; set; }

	/// <summary>
	/// Sub-playlist that the song should show up in
	/// </summary>
	[IniKeySerializable(IniFormatting.SubPlaylist)]
	public string? SubPlaylist { get; set; }

	/// <summary>
	/// Track number of the song within the playlist/setlist
	/// </summary>
	[IniKeySerializable(IniFormatting.PlaylistTrack)]
	public ushort? PlaylistTrack { get; set; }

	/// <summary>
	/// Year of release
	/// </summary>
	[IniKeySerializable(IniFormatting.Year)]
	public ushort? Year { get; set; }

	/// <summary>
	/// Genre of the <see cref="Song"/>
	/// </summary>
	[ChartKeySerializable(ChartFormatting.Genre)]
	[IniKeySerializable(IniFormatting.Genre)]
	public string? Genre { get; set; }

	/// <summary>
	/// Creator of the chart
	/// </summary>
	public Charter Charter
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			field = value;
		}
	} = new();

	/// <summary>
	/// The song contains explicit lyrics
	/// </summary>
	[IniKeySerializable(IniFormatting.Explicit)]
	public bool? Explicit { get; set; }

	/// <summary>
	/// Start time in milliseconds of the preview in the Clone Hero song browser
	/// </summary>
	[ChartKeySerializable(ChartFormatting.PreviewStart)]
	[IniKeySerializable(IniFormatting.PreviewStart)]
	public uint? PreviewStart { get; set; }

	/// <summary>
	/// End time in milliseconds of the preview in the Clone Hero song browser
	/// </summary>
	[ChartKeySerializable(ChartFormatting.PreviewEnd)]
	[IniKeySerializable(IniFormatting.PreviewEnd)]
	public uint? PreviewEnd { get; set; }

	/// <summary>
	/// Duration in milliseconds of the preview in the Clone Hero song browser
	/// </summary>
	public uint PreviewLength
	{
		get
		{
			if (PreviewEnd is null)
				return 30000;

			return PreviewStart is null ? PreviewEnd.Value : PreviewEnd.Value - PreviewStart.Value;
		}
	}

	/// <summary>
	/// Overall difficulty of the song
	/// </summary>
	[ChartKeySerializable(ChartFormatting.Difficulty)]
	[IniKeySerializable(IniFormatting.Difficulties.Global)]
	public sbyte? Difficulty { get; set; }

	/// <inheritdoc cref="InstrumentDifficultySet"/>
	public InstrumentDifficultySet InstrumentDifficulties
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			field = value;
		}
	} = new();

	/// <summary>
	/// Type of media the audio track comes from
	/// </summary>
	[ChartKeySerializable(ChartFormatting.MediaType)]
	public string? MediaType { get; set; }

	/// <summary>
	/// Offset of the audio track. A higher value makes the audio start sooner.
	/// </summary>
	[ChartKeySerializable(ChartFormatting.AudioOffset)]
	[IniKeySerializable(IniFormatting.AudioOffset)]
	public TimeSpan? AudioOffset { get; set; }

	/// <summary>
	/// Paths of audio files
	/// </summary>
	public StreamCollection Streams
	{
		get;
		set
		{
			ArgumentNullException.ThrowIfNull(value);
			field = value;
		}
	} = new();

	/// <summary>
	/// Offset of the background video. A higher value makes the video start sooner.
	/// </summary>
	public TimeSpan? VideoOffset { get; set; }

	/// <summary>
	/// Length of the song in milliseconds
	/// </summary>
	[IniKeySerializable(IniFormatting.Length)]
	public uint? Length { get; set; }

	/// <summary>
	/// Text to be displayed on the load screen
	/// </summary>
	[IniKeySerializable(IniFormatting.LoadingText)]
	public string? LoadingText { get; set; }

	/// <summary>
	/// The song is a modchart
	/// </summary>
	[IniKeySerializable(IniFormatting.Modchart)]
	public bool? IsModchart { get; set; }

	/// <inheritdoc cref="FormattingRules"/>
	public FormattingRules Formatting
	{
		get;
		set => field = value ?? throw new ArgumentNullException(nameof(value));
	} = new();

	/// <summary>
	/// Unrecognized metadata
	/// </summary>
	/// <remarks>When writing, these will only be written if the target format matches the origin</remarks>
	public HashSet<UnidentifiedMetadata> UnidentifiedData { get; } =
		new(new FuncEqualityComparer<UnidentifiedMetadata>(
			static (a, b) => a.Key == b.Key && a.Origin == b.Origin));
	#endregion

	public string? Get(FileType fileType, string key)
		=> fileType switch
	{
		FileType.Chart => MetadataChartMapper.Get(this, key),
		FileType.Ini   => MetadataIniMapper.Get(this, key)
	};

	public void Set(FileType fileType, string key, string value)
	{
		switch (fileType)
		{
			case FileType.Chart:
				MetadataChartMapper.Set(this, key, value);
				break;
			case FileType.Ini:
				MetadataIniMapper.Set(this, key, value);
				break;
		}
	}

	public void Remove(FileType fileType, string key)
	{
		switch (fileType)
		{
			case FileType.Chart:
				MetadataChartMapper.Remove(this, key);
				break;
			case FileType.Ini:
				throw new NotImplementedException();
		}
	}

	/// <summary>
	/// Appends the metadata from another file.
	/// </summary>
	/// <param name="path">Path of the file to read</param>
	public void ReadFile(string path)
		=> Read(path, this);

	/// <summary>
	/// Reads the <see cref="Metadata"/> from a file.
	/// </summary>
	/// <param name="path">Path of the file to read</param>
	public static Metadata FromFile(string path)
		=> Read(path);

	private static Metadata Read(string path, Metadata? existing = null)
		=> ExtensionHandler.Read(path,
			(".chart", static p => ChartFile.ReadMetadata(p)),
			(".ini", path => IniFile.ReadMetadata(path, existing)));

	/// <summary>
	/// Reads the <see cref="Metadata"/> from multiple files.
	/// </summary>
	/// <remarks>Each file has less priority than the preceding.</remarks>
	/// <param name="paths">Paths of the files to read</param>
	public static Metadata? FromFiles(params ReadOnlySpan<string> paths)
	{
		// No files provided
		if (paths.Length == 0)
			throw new ArgumentException("No provided paths");

		Metadata data = FromFile(paths[0]);

		foreach (string path in paths[1..])
			data.ReadFile(path);

		return data;
	}

	/// <summary>
	/// Writes the <see cref="Metadata"/> to a file.
	/// </summary>
	/// <param name="path">Path of the file to write</param>
	public void ToFile(string path)
		=> ExtensionHandler.Write(path, this,
			(".chart", static (p, m) => ChartFile.ReplaceMetadata(p, m)),
			(".ini", static (p, m) => IniFile.WriteMetadata(p, m)));
}
