using ChartTools.IO;
using ChartTools.IO.Chart;

namespace ChartTools.Meta.Mapping;

internal class MetadataChartMapper : IMetadataMapper
{
	public static FileType FileType => FileType.Chart;

	public static string? Get(Metadata metadata, in ReadOnlySpan<char> key)
		=> key switch
		{
			ChartFormatting.Title        => metadata.Title,
			ChartFormatting.Artist       => metadata.Artist,
			ChartFormatting.Charter      => metadata.Charter.Name,
			ChartFormatting.Album        => metadata.Album,
			ChartFormatting.Year         => metadata.Year is null ? null : $"\", {metadata.Year}\"",
			ChartFormatting.AudioOffset  => metadata.AudioOffset?.TotalMilliseconds.ToString(),
			ChartFormatting.Difficulty   => metadata.Difficulty?.ToString(),
			ChartFormatting.PreviewStart => metadata.PreviewStart?.ToString(),
			ChartFormatting.PreviewEnd   => metadata.PreviewEnd?.ToString(),
			ChartFormatting.Genre        => metadata.Genre,
			ChartFormatting.MediaType    => metadata.MediaType,
			ChartFormatting.MusicStream  => metadata.Streams.Music,
			ChartFormatting.GuitarStream => metadata.Streams.Guitar,
			ChartFormatting.BassStream   => metadata.Streams.Bass,
			ChartFormatting.RhythmStream => metadata.Streams.Rhythm,
			ChartFormatting.KeysStream   => metadata.Streams.Keys,
			ChartFormatting.DrumStream   => metadata.Streams.Drum,
			ChartFormatting.Drum2Stream  => metadata.Streams.Drum2,
			ChartFormatting.Drum3Stream  => metadata.Streams.Drum3,
			ChartFormatting.Drum4Stream  => metadata.Streams.Drum4,
			ChartFormatting.VocalStream  => metadata.Streams.Vocals,
			ChartFormatting.CrowdStream  => metadata.Streams.Crowd,
			_ => IMetadataMapper.FindUndentified(metadata, FileType.Chart, key)
		};

	public static void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		switch (key)
		{
			case ChartFormatting.Title:
				metadata.Title = value.ToString();
				break;
			case ChartFormatting.Artist:
				metadata.Artist = value.ToString();
				break;
			case ChartFormatting.Charter:
				metadata.Charter.Name = value.ToString();
				break;
			case ChartFormatting.Album:
				metadata.Album = value.ToString();
				break;
			case ChartFormatting.Year:
				metadata.Year = ValueParser.ParseUshort(value.TrimStart(','), "year");
				break;
			case ChartFormatting.AudioOffset:
				metadata.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.ParseFloat(value, "audio offset") * 1000);
				break;
			case ChartFormatting.Difficulty:
				metadata.Difficulty = ValueParser.ParseSbyte(value, "difficulty");
				break;
			case ChartFormatting.PreviewStart:
				metadata.PreviewStart = ValueParser.ParseUint(value, "preview start");
				break;
			case ChartFormatting.PreviewEnd:
				metadata.PreviewEnd = ValueParser.ParseUint(value, "preview end");
				break;
			case ChartFormatting.Genre:
				metadata.Genre = value.ToString();
				break;
			case ChartFormatting.MediaType:
				metadata.MediaType = value.ToString();
				break;
			case ChartFormatting.MusicStream:
				metadata.Streams.Music = value.ToString();
				break;
			case ChartFormatting.GuitarStream:
				metadata.Streams.Guitar = value.ToString();
				break;
			case ChartFormatting.BassStream:
				metadata.Streams.Bass = value.ToString();
				break;
			case ChartFormatting.RhythmStream:
				metadata.Streams.Rhythm = value.ToString();
				break;
			case ChartFormatting.KeysStream:
				metadata.Streams.Keys = value.ToString();
				break;
			case ChartFormatting.DrumStream:
				metadata.Streams.Drum = value.ToString();
				break;
			case ChartFormatting.Drum2Stream:
				metadata.Streams.Drum2 = value.ToString();
				break;
			case ChartFormatting.Drum3Stream:
				metadata.Streams.Drum3 = value.ToString();
				break;
			case ChartFormatting.Drum4Stream:
				metadata.Streams.Drum4 = value.ToString();
				break;
			case ChartFormatting.VocalStream:
				metadata.Streams.Vocals = value.ToString();
				break;
			case ChartFormatting.CrowdStream:
				metadata.Streams.Crowd = value.ToString();
				break;
			default:
				metadata.UnidentifiedData.Add(new()
				{
					Key    = key.ToString(),
					Value  = value.ToString(),
					Origin = FileType.Chart
				});
				break;
		}
	}

	public static void Remove(Metadata metadata, in ReadOnlySpan<char> key)
		=> IMetadataMapper.Remove(metadata, FileType.Chart, in key);

	private MetadataChartMapper() { }
}
