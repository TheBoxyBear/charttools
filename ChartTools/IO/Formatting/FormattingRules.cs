using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.Meta;

namespace ChartTools.IO.Formatting;

/// <summary>
/// Rules defined in song.ini that affect how the song data file is read and written
/// </summary>
/// <remarks>Property summaries provided by Nathan Hurst.</remarks>
public sealed class FormattingRules
{
	public AlbumTrackKeys AlbumTrackKey
	{
		get;
		set => field = value == 0 ? DefaultAlbumTrackKey : value;
	}

	public AlbumTrackKeys DefaultAlbumTrackKey => AlbumTrackKeys.Track;

	public CharterKeys CharterKey
	{
		get;
		set => field = value == 0 ? DefaultCharterKey : value;
	}

	public CharterKeys DefaultCharterKey => CharterKeys.Charter;

	/// <summary>
	/// Number of <see cref="ITrackObject.Position"/> values per beat
	/// </summary>
	[MetadataKey(FileType.Chart, ChartFormatting.Resolution)]
	public uint? Resolution { get; set; }

	public uint TrueResolution => Resolution ?? 480;

	/// <summary>
	/// Overrides the default sustain cutoff threshold with the specified number of ticks.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.SustainCutoff)]
	public uint? SustainCutoff { get; set; }

	/// <summary>
	/// Overrides the natural HOPO threshold with the specified number of ticks.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.HopoFrequency)]
	public uint? HopoFrequency { get; set; }

	internal uint ChartHopoFrequency => (uint)(65 / 192f * TrueResolution);

	#region Star power
	/// <summary>
	/// Overrides the Star Power phrase MIDI note for .mid charts.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.MultiplierNote)]
	public byte? MultiplierNote { get; set; }

	/// <summary>
	/// (PhaseShift) Overrides the Star Power phrase MIDI note for .mid charts.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.StarPowerNote)]
	public byte? StarPowerNote { get; set; }

	public byte? TrueStarPowerNote => StarPowerNote ?? MultiplierNote;
	#endregion

	#region SysEx
	/// <summary>
	/// (PhaseShift) Indicates if the chart uses SysEx events for sliders/tap notes.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.SysExSliders)]
	public bool? SysExSliders { get; set; }

	/// <summary>
	/// (PhaseShift) Indicates if the chart uses SysEx events for Drums Real hi-hat pedal control.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.SysExHighHat)]
	public bool? SysExHighHat { get; set; }

	/// <summary>
	/// (PhaseShift) Indicates if the chart uses SysEx events for Drums Real rimshot hits.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.Rimshot)]
	public bool? SysExRimshot { get; set; }

	/// <summary>
	/// (PhaseShift) Indicates if the chart uses SysEx events for open notes.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.SysExOpenBass)]
	public bool? SysExOpenBass { get; set; }

	/// <summary>
	/// (PhaseShift) Indicates if the chart uses SysEx events for Pro Guitar/Bass slide directions.
	/// </summary>
	[MetadataKey(FileType.Ini, IniFormatting.SysExProSlide)]
	public bool? SysexProSlide { get; set; }
	#endregion
}
