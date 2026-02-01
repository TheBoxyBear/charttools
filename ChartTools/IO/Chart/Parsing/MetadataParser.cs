using ChartTools.Meta;
using ChartTools.Meta.Mapping;

internal class MetadataParser(Metadata? existing = null)
	: ChartParser(null! /* Session not used */, ChartFormatting.MetadataHeader.AsMemory())
{
	public override Metadata Result
		=> GetResult(m_result);

	private readonly Metadata m_result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);
		ReadOnlySpan<char> value = entry.Value.Span.Trim('"');

		switch (entry.Key.Span)
		{
			case ChartFormatting.Title:
				m_result.Title = value.ToString();
				break;
			case ChartFormatting.Artist:
				m_result.Artist = value.ToString();
				break;
			case ChartFormatting.Charter:
				m_result.Charter.Name = value.ToString();
				break;
			case ChartFormatting.Album:
				m_result.Album = value.ToString();
				break;
			case ChartFormatting.Year:
				m_result.Year = ValueParser.Parse<ushort>(value.TrimStart(','), "year");
				break;
			case ChartFormatting.AudioOffset:
				m_result.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<float>(value, "audio offset") * 1000);
				break;
			case ChartFormatting.Difficulty:
				m_result.Difficulty = ValueParser.Parse<sbyte>(value, "difficulty");
				break;
			case ChartFormatting.PreviewStart:
				m_result.PreviewStart = ValueParser.Parse<uint>(value, "preview start");
				break;
			case ChartFormatting.PreviewEnd:
				m_result.PreviewEnd = ValueParser.Parse<uint>(value, "preview end");
				break;
			case ChartFormatting.Genre:
				m_result.Genre = value.ToString();
				break;
			case ChartFormatting.MediaType:
				m_result.MediaType = value.ToString();
				break;
			case ChartFormatting.MusicStream:
				m_result.Streams.Music = value.ToString();
				break;
			case ChartFormatting.GuitarStream:
				m_result.Streams.Guitar = value.ToString();
				break;
			case ChartFormatting.BassStream:
				m_result.Streams.Bass = value.ToString();
				break;
			case ChartFormatting.RhythmStream:
				m_result.Streams.Rhythm = value.ToString();
				break;
			case ChartFormatting.KeysStream:
				m_result.Streams.Keys = value.ToString();
				break;
			case ChartFormatting.DrumStream:
				m_result.Streams.Drum = value.ToString();
				break;
			case ChartFormatting.Drum2Stream:
				m_result.Streams.Drum2 = value.ToString();
				break;
			case ChartFormatting.Drum3Stream:
				m_result.Streams.Drum3 = value.ToString();
				break;
			case ChartFormatting.Drum4Stream:
				m_result.Streams.Drum4 = value.ToString();
				break;
			case ChartFormatting.VocalStream:
				m_result.Streams.Vocals = value.ToString();
				break;
			case ChartFormatting.CrowdStream:
				m_result.Streams.Crowd = value.ToString();
				break;
			default:
				m_result.UnidentifiedData.Add(new() { Key = entry.Key.ToString(), Value = value.ToString(), Origin = FileType.Chart });
				break;
		}
	}

	public override void ApplyToSong(Song song)
		=> song.Metadata = Result;
}
