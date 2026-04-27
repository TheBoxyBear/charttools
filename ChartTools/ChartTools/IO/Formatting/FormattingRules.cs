using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.Meta;
using ChartTools.Extensions.Enums;

namespace ChartTools.IO.Formatting;

/// <summary>
/// Rules defined in song.ini that affect how the song data file is read and written
/// </summary>
/// <remarks>Property summaries provided by Nathan Hurst.</remarks>
public sealed class FormattingRules
{
	#region Album track
	public SafeEnum<AlbumTrackKeys> AlbumTrackKeys { get; set; }
		= SafeEnum.Unsafe(Formatting.AlbumTrackKeys.None);

	public static SafeEnum<AlbumTrackKeys> DefaultAlbumTrackKeys
	{
		get;
		set
		{
			if (value == Formatting.AlbumTrackKeys.None)
				throw new ArgumentException($"Default {nameof(AlbumTrackKeys)} must have a flag.", nameof(value));

			field = value;
		}
	} = SafeEnum.Unsafe(Formatting.AlbumTrackKeys.Track);

	public SafeEnum<AlbumTrackKeys> EffectiveAlbumTrackKeys
		=> AlbumTrackKeys == Formatting.AlbumTrackKeys.None ? DefaultAlbumTrackKeys : AlbumTrackKeys;
	#endregion

	#region Charter
	public SafeEnum<CharterKeys> CharterKeys { get; set; }
		= SafeEnum.Unsafe(Formatting.CharterKeys.None);

	public static SafeEnum<CharterKeys> DefaultCharterKey
	{
		get;
		set
		{
			if (value == Formatting.CharterKeys.None)
				throw new ArgumentException($"Default {nameof(CharterKeys)} must have a flag.", nameof(value));

			field = value;
		}
	} = SafeEnum.Unsafe(Formatting.CharterKeys.Charter);

	public SafeEnum<CharterKeys> EffectiveCharterKeys
		=> CharterKeys == Formatting.CharterKeys.None ? DefaultCharterKey : CharterKeys;
	#endregion

	/// <summary>
	/// Number of <see cref="ITrackObject.Position"/> values per beat
	/// </summary>
	[MetadataKey(FileType.Chart, ChartFormatting.Resolution)]
	public uint? Resolution { get; set; }

	public uint EffectiveResolution => Resolution ?? 480;

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

	internal uint ChartHopoFrequency => (uint)(65 / 192f * EffectiveResolution);

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

	public byte? EffectiveStarPowerNote => StarPowerNote ?? MultiplierNote;
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
