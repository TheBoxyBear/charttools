namespace ChartTools.IO.Chart.Parsing;

internal class MetadataParser(Metadata? existing = null)
	: ChartParser(null! /* Session not used */, ChartFormatting.MetadataHeader.AsMemory())
{
	public override Metadata Result => GetResult(result);
	private readonly Metadata result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);
		ReadOnlySpan<char> value = entry.Value.Trim('"').Span;

		switch (entry.Key.Span)
		{
			case ChartFormatting.Title:
				result.Title = value.ToString();
				break;
			case ChartFormatting.Artist:
				result.Artist = value.ToString();
				break;
			case ChartFormatting.Charter:
				result.Charter.Name = value.ToString();
				break;
			case ChartFormatting.Album:
				result.Album = value.ToString();
				break;
			case ChartFormatting.Year:
				result.Year = ValueParser.ParseUshort(value.TrimStart(','), "year");
				break;
			case ChartFormatting.AudioOffset:
				result.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.ParseFloat(value, "audio offset") * 1000);
				break;
			case ChartFormatting.Difficulty:
				result.Difficulty = ValueParser.ParseSbyte(value, "difficulty");
				break;
			case ChartFormatting.PreviewStart:
				result.PreviewStart = ValueParser.ParseUint(value, "preview start");
				break;
			case ChartFormatting.PreviewEnd:
				result.PreviewEnd = ValueParser.ParseUint(value, "preview end");
				break;
			case ChartFormatting.Genre:
				result.Genre = value.ToString();
				break;
			case ChartFormatting.MediaType:
				result.MediaType = value.ToString();
				break;
			case ChartFormatting.MusicStream:
				result.Streams.Music = value.ToString();
				break;
			case ChartFormatting.GuitarStream:
				result.Streams.Guitar = value.ToString();
				break;
			case ChartFormatting.BassStream:
				result.Streams.Bass = value.ToString();
				break;
			case ChartFormatting.RhythmStream:
				result.Streams.Rhythm = value.ToString();
				break;
			case ChartFormatting.KeysStream:
				result.Streams.Keys = value.ToString();
				break;
			case ChartFormatting.DrumStream:
				result.Streams.Drum = value.ToString();
				break;
			case ChartFormatting.Drum2Stream:
				result.Streams.Drum2 = value.ToString();
				break;
			case ChartFormatting.Drum3Stream:
				result.Streams.Drum3 = value.ToString();
				break;
			case ChartFormatting.Drum4Stream:
				result.Streams.Drum4 = value.ToString();
				break;
			case ChartFormatting.VocalStream:
				result.Streams.Vocals = value.ToString();
				break;
			case ChartFormatting.CrowdStream:
				result.Streams.Crowd = value.ToString();
				break;
			default:
				result.UnidentifiedData.Add(new()
				{
					Key    = entry.Key.ToString(),
					Value  = entry.Value.ToString(),
					Origin = FileType.Chart
				});
				break;
		}
	}

	public override void ApplyToSong(Song song) => song.Metadata = Result;
}
