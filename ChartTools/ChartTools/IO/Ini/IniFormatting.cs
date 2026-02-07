using ChartTools.Meta;

namespace ChartTools.IO.Ini;

/// <summary>
/// Provides a set of helper strings for working with Ini files
/// </summary>
public static class IniFormatting
{
	/// <summary>
	/// Start of the ini section containing song metadata
	/// </summary>
	public const string Header = "[song]";

	/// <summary>
	/// Value of <see cref="Metadata.Title"/>
	/// </summary>
	public const string Title = "name";

	/// <summary>
	/// Value of <see cref="Metadata.Artist"/>
	/// </summary>
	public const string Artist = "artist";

	/// <summary>
	/// Value of <see cref="Metadata.Album"/>
	/// </summary>
	public const string Album = "album";

	/// <summary>
	/// Value of <see cref="Metadata.AlbumTrack"/>
	/// </summary>
	/// <remarks>Alternate key to <see cref="Track"/></remarks>
	public const string AlbumTrack = "album_track";

	/// <summary>
	/// Value of <see cref="Metadata.AlbumTrack"/>
	/// </summary>
	/// <remarks>Alternate key to <see cref="AlbumTrack"/></remarks>
	public const string Track = "track";

	/// <summary>
	/// Value of <see cref="Metadata.Playlist"/>
	/// </summary>
	public const string Playlist = "playlist";

	/// <summary>
	/// Value of <see cref="Metadata.SubPlaylist"/>
	/// </summary>
	public const string SubPlaylist = "sub_playlist";

	/// <summary>
	/// Value of <see cref="Metadata.PlaylistTrack"/>
	/// </summary>
	public const string PlaylistTrack = "playlist_track";

	/// <summary>
	/// Value of <see cref="Metadata.Genre"/>
	/// </summary>
	public const string Genre = "genre";

	/// <summary>
	/// Value of <see cref="Metadata.Explicit"/>
	/// </summary>
	public const string Explicit = "explicit_lyrics";

	/// <summary>
	/// Value of <see cref="Metadata.Year"/>
	/// </summary>
	public const string Year = "year";

	/// <summary>
	/// Value of <see cref="Charter.Name"/>
	/// </summary>
	/// <remarks>Alternate key to <see cref="Frets"/></remarks>
	public const string Charter = "charter";

	/// <summary>
	/// Value of <see cref="Charter.Name"/>
	/// </summary>
	/// <remarks>Alternate key to <see cref="Charter"/></remarks>
	public const string Frets = "frets";

	/// <summary>
	/// Value of <see cref="Charter.Icon"/>
	/// </summary>
	public const string Icon = "icon";

	/// <summary>
	/// Value of <see cref="Metadata.PreviewStart"/>
	/// </summary>
	public const string PreviewStart = "preview_start_time";

	/// <summary>
	/// Value of <see cref="Metadata.PreviewEnd"/>
	/// </summary>
	public const string PreviewEnd = "preview_end_time";

	/// <summary>
	/// Value of <see cref="Metadata.AudioOffset"/>
	/// </summary>
	public const string AudioOffset = "delay";

	/// <summary>
	/// Value of <see cref="Metadata.VideoOffset"/>
	/// </summary>
	public const string VideoOffset = "video_start_time";

	/// <summary>
	/// Value of <see cref="Metadata.Length"/>
	/// </summary>
	public const string Length = "song_length";

	/// <summary>
	/// Value of <see cref="Metadata.LoadingText"/>
	/// </summary>
	public const string LoadingText = "loading_text";

	/// <summary>
	/// Value of <see cref="Metadata.IsModchart"/>
	/// </summary>
	public const string Modchart = "modchart";

	/// <summary>
	/// Value of <see cref="Formatting.FormattingRules.SustainCutoff"/>
	/// </summary>
	public const string SustainCutoff = "sustain_cutoff_threshold";

	/// <summary>
	/// Value of <see cref="Formatting.FormattingRules.HopoFrequency"/>
	/// </summary>
	public const string HopoFrequency = "hopo_frequency";

	public const string MultiplierNote = "multiplier_note";
	public const string StarPowerNote = "star_power_note";
	public const string SysExSliders = "sysex_slider";
	public const string SysExHighHat = "sysex_high_hat_ctrl";
	public const string Rimshot = "sysex_rimshot";
	public const string SysExOpenBass = "sysex_open_bass";
	public const string SysExProSlide = "sysex_pro_slide";

	/// <summary>
	/// Value of <see cref="InstrumentDifficultySet.GHLCoopGuitar"/>
	/// </summary>
	public const string GHLCoopGuitarDifficulty = "diff_guitar_coop_ghl";

	public static string Line(in ReadOnlySpan<char> key, ReadOnlySpan<char> value)
		=> $"{key} = {value}";

	public class Difficulties
	{
		/// <summary>
		/// Value of <see cref="Metadata.Difficulty"/>
		/// </summary>
		public const string Global = "diff_band";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.StandardLeadGuitar"/>
		/// </summary>
		public const string StandardLeadGuitar = "diff_guitar";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.StandardRhythmGuitar"/>
		/// </summary>
		public const string StandardRhythmGuitar = "diff_rhythm";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.StandardCoopGuitar"/>
		/// </summary>
		public const string StandardCoopGuitar = "diff_guitar_coop";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.StandardBass"/>
		/// </summary>
		public const string StandardBass = "diff_bass";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.Drums"/>
		/// </summary>
		public const string Drums = "diff_drums";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.StandardKeys"/>
		/// </summary>
		public const string StandardKeys = "diff_keys";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.GHLLeadGuitar"/>
		/// </summary>
		public const string GHLLeadGuitar = "diff_guitarghl";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.GHLRhythmGuitar"/>
		/// </summary>
		public const string GHLRhythmGuitar = "diff_rhythm_ghl";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.GHLCoopGuitar"/>
		/// </summary>
		public const string GHLCoopGuitar = "diff_guitar_coop_ghl";

		/// <summary>
		/// Value of <see cref="InstrumentDifficultySet.GHLBass"/>
		/// </summary>
		public const string GHLBass = "diff_bassghl";
	}
}
