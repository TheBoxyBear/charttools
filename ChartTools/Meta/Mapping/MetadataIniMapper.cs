using ChartTools.IO;
using ChartTools.IO.Ini;

namespace ChartTools.Meta.Mapping;

internal class MetadataIniMapper : IMetadataMapper
{
	public static FileType FileType => FileType.Ini;

	public static string? Get(Metadata metadata, in ReadOnlySpan<char> key)
		=> key switch
	{
		IniFormatting.Title                             => metadata.Title,
		IniFormatting.Artist                            => metadata.Artist,
		IniFormatting.Album                             => metadata.Charter.Name,
		IniFormatting.AlbumTrack                        => metadata.AlbumTrack?.ToString(),
		IniFormatting.Track                             => metadata.AlbumTrack?.ToString(),
		IniFormatting.Playlist                          => metadata.Playlist,
		IniFormatting.SubPlaylist                       => metadata.SubPlaylist,
		IniFormatting.PlaylistTrack                     => metadata.PlaylistTrack?.ToString(),
		IniFormatting.Year                              => metadata.Year?.ToString(),
		IniFormatting.Genre                             => metadata.Genre,
		IniFormatting.Charter                           => metadata.Charter.Name,
		IniFormatting.Frets                             => metadata.Charter.Name,
		IniFormatting.Icon                              => metadata.Charter.Icon,
		IniFormatting.PreviewStart                      => metadata.PreviewStart?.ToString(),
		IniFormatting.PreviewEnd                        => metadata.PreviewEnd?.ToString(),
		IniFormatting.AudioOffset                       => metadata.AudioOffset?.TotalMilliseconds.ToString(),
		IniFormatting.VideoOffset                       => metadata.VideoOffset?.TotalMilliseconds.ToString(),
		IniFormatting.Length                            => metadata.Length?.ToString(),
		IniFormatting.LoadingText                       => metadata.LoadingText,
		IniFormatting.Modchart                          => metadata.IsModchart.HasValue ? (metadata.IsModchart.Value ? "1" : "0") : null,
		IniFormatting.Explicit                          => metadata.Explicit?.ToString(),
		IniFormatting.Difficulties.Global               => metadata.Difficulty?.ToString(),
		IniFormatting.Difficulties.StandardLeadGuitar   => metadata.InstrumentDifficulties.StandardLeadGuitar?.ToString(),
		IniFormatting.Difficulties.StandardRhythmGuitar => metadata.InstrumentDifficulties.StandardRhythmGuitar?.ToString(),
		IniFormatting.Difficulties.StandardCoopGuitar   => metadata.InstrumentDifficulties.StandardCoopGuitar?.ToString(),
		IniFormatting.Difficulties.StandardBass         => metadata.InstrumentDifficulties.StandardBass?.ToString(),
		IniFormatting.Difficulties.Drums                => metadata.InstrumentDifficulties.Drums?.ToString(),
		IniFormatting.Difficulties.StandardKeys         => metadata.InstrumentDifficulties.StandardKeys?.ToString(),
		IniFormatting.Difficulties.GHLLeadGuitar        => metadata.InstrumentDifficulties.GHLLeadGuitar?.ToString(),
		IniFormatting.Difficulties.GHLRhythmGuitar      => metadata.InstrumentDifficulties.GHLRhythmGuitar?.ToString(),
		IniFormatting.Difficulties.GHLCoopGuitar        => metadata.InstrumentDifficulties.GHLCoopGuitar?.ToString(),
		IniFormatting.Difficulties.GHLBass              => metadata.InstrumentDifficulties.GHLBass?.ToString(),
		IniFormatting.SustainCutoff                     => metadata.Formatting.SustainCutoff?.ToString(),
		IniFormatting.HopoFrequency                     => metadata.Formatting.HopoFrequency?.ToString(),
		_                                               => IMetadataMapper.FindUndentified(metadata, FileType.Ini, key)
	};

	public static void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		// TODO Implement with source generation
		throw new NotImplementedException();
	}

	public static void Remove(Metadata metadata, in ReadOnlySpan<char> key)
		=> IMetadataMapper.Remove(metadata, FileType.Ini, in key);

	private MetadataIniMapper() { }
}
