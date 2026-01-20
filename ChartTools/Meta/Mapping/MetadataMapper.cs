using ChartTools.IO.Chart;

namespace ChartTools.Meta.Mapping;

internal abstract class MetadataMapper
{
	public abstract FileType FileType { get; }

	public abstract string? Get(Metadata metadata, in ReadOnlySpan<char> key);

	public abstract void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	public virtual void Remove(Metadata metadata, in ReadOnlySpan<char> key)
	{
		switch (key)
		{
			case ChartFormatting.Title:
				metadata.Title = null;
				break;
			case ChartFormatting.Artist:
				metadata.Artist = null;
				break;
			case ChartFormatting.Charter:
				metadata.Charter.Name = null;
				break;
			case ChartFormatting.Album:
				metadata.Album = null;
				break;
			case ChartFormatting.Year:
				metadata.Year = null;
				break;
			case ChartFormatting.AudioOffset:
				metadata.AudioOffset = null;
				break;
			case ChartFormatting.Difficulty:
				metadata.Difficulty = null;
				break;
			case ChartFormatting.PreviewStart:
				metadata.PreviewStart = null;
				break;
			case ChartFormatting.PreviewEnd:
				metadata.PreviewEnd = null;
				break;
			case ChartFormatting.Genre:
				metadata.Genre = null;
				break;
			case ChartFormatting.MediaType:
				metadata.MediaType = null;
				break;
			case ChartFormatting.MusicStream:
				metadata.Streams.Music = null;
				break;
			case ChartFormatting.GuitarStream:
				metadata.Streams.Guitar = null;
				break;
			case ChartFormatting.BassStream:
				metadata.Streams.Bass = null;
				break;
			case ChartFormatting.RhythmStream:
				metadata.Streams.Rhythm = null;
				break;
			case ChartFormatting.KeysStream:
				metadata.Streams.Keys = null;
				break;
			case ChartFormatting.DrumStream:
				metadata.Streams.Drum = null;
				break;
			case ChartFormatting.Drum2Stream:
				metadata.Streams.Drum2 = null;
				break;
			case ChartFormatting.Drum3Stream:
				metadata.Streams.Drum3 = null;
				break;
			case ChartFormatting.Drum4Stream:
				metadata.Streams.Drum4 = null;
				break;
			case ChartFormatting.VocalStream:
				metadata.Streams.Vocals = null;
				break;
			case ChartFormatting.CrowdStream:
				metadata.Streams.Crowd = null;
				break;
			default:
				metadata.UnidentifiedData.Remove(new()
				{
					Key = key.ToString(),
					Origin = FileType
				});
				break;
		}
	}

	protected string? FindUndentified(Metadata metadata, in ReadOnlySpan<char> key)
		=> metadata.UnidentifiedData.TryGetValue(new()
		{
			Key    = key.ToString(),
			Origin = FileType
		}, out UnidentifiedMetadata found)
			? found.Value
			: null;
}
