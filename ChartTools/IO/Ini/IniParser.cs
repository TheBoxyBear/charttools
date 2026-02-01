using ChartTools.IO.Formatting;
using ChartTools.IO.Parsing;
using ChartTools.Tools;

namespace ChartTools.IO.Ini;

internal class IniParser(Metadata? existing = null) : TextParser(IniFormatting.Header.AsMemory()), ISongAppliable
{
	public override Metadata Result
		=> GetResult(m_result);

	private readonly Metadata m_result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);

		if (entry.Value.Length is 0)
			return;

		ReadOnlySpan<char> value = entry.Value.Span;

		switch (entry.Key.Span)
		{
			case IniFormatting.Title:
				m_result.Title = value.ToString();
				break;
			case IniFormatting.Artist:
				m_result.Artist = value.ToString();
				break;
			case IniFormatting.Album:
				m_result.Album = value.ToString();
				break;
			case IniFormatting.AlbumTrack:
				ParseAlbumTrack(in value);
				m_result.Formatting.AlbumTrackKey |= AlbumTrackKey.AlbumTrack;
				break;
			case IniFormatting.Track:
				ParseAlbumTrack(in value);
				m_result.Formatting.AlbumTrackKey |= AlbumTrackKey.Track;
				break;
			case IniFormatting.Playlist:
				m_result.Playlist = value.ToString();
				break;
			case IniFormatting.SubPlaylist:
				m_result.SubPlaylist = value.ToString();
				break;
			case IniFormatting.PlaylistTrack:
				m_result.PlaylistTrack = ValueParser.Parse<ushort>(value, "playlist track");
				break;
			case IniFormatting.Year:
				m_result.Year = ValueParser.Parse<ushort>(value, "year");
				break;
			case IniFormatting.Genre:
				m_result.Genre = value.ToString();
				break;
			case IniFormatting.Charter:
				ParseCharter(in value);
				m_result.Formatting.CharterKey |= CharterKey.Charter;
				break;
			case IniFormatting.Frets:
				ParseCharter(in value);
				m_result.Formatting.CharterKey |= CharterKey.Frets;
				break;
			case IniFormatting.Icon:
				m_result.Charter.Icon = value.ToString();
				break;
			case IniFormatting.PreviewStart:
				m_result.PreviewStart = value.StartsWith('-') ? null : ValueParser.Parse<uint>(value, "preview start");
				break;
			case IniFormatting.PreviewEnd:
				m_result.PreviewEnd = value.StartsWith('-') ? null : ValueParser.Parse<uint>(value, "preview end");
				break;
			case IniFormatting.AudioOffset:
				m_result.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<int>(value.ToString(), "audio offset"));
				break;
			case IniFormatting.VideoOffset:
				m_result.VideoOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<int>(value, "video offset"));
				break;
			case IniFormatting.Length:
				m_result.Length = ValueParser.Parse<uint>(value, "song length");
				break;
			case IniFormatting.LoadingText:
				m_result.LoadingText = value.ToString();
				break;
			case IniFormatting.Modchart:
				m_result.IsModchart = ValueParser.Parse<int>(value, "modchart") == 1;
				break;
			case IniFormatting.Explicit:
				m_result.Explicit = ValueParser.Parse<bool>(value, "explicit");
				break;
			case IniFormatting.Difficulties.Global:
				m_result.Difficulty = ValueParser.Parse<sbyte>(value, "difficulty");
				break;
			case IniFormatting.Difficulties.StandardLeadGuitar:
				m_result.InstrumentDifficulties.StandardLeadGuitar = ValueParser.Parse<sbyte>(value, "lead guitar difficulty");
				break;
			case IniFormatting.Difficulties.StandardRhythmGuitar:
				m_result.InstrumentDifficulties.StandardRhythmGuitar = ValueParser.Parse<sbyte>(value, "rhythm guitar difficulty");
				break;
            case IniFormatting.Difficulties.StandardCoopGuitar:
                m_result.InstrumentDifficulties.StandardCoopGuitar = ValueParser.Parse<sbyte>(value, "coop guitar difficulty");
                break;
            case IniFormatting.Difficulties.StandardBass:
				m_result.InstrumentDifficulties.StandardBass = ValueParser.Parse<sbyte>(value, "bass difficulty");
				break;
			case IniFormatting.Difficulties.Drums:
				m_result.InstrumentDifficulties.Drums = ValueParser.Parse<sbyte>(value, "drums difficulty");
				break;
			case IniFormatting.Difficulties.StandardKeys:
				m_result.InstrumentDifficulties.StandardKeys = ValueParser.Parse<sbyte>(value, "keys difficulty");
				break;
            case IniFormatting.Difficulties.GHLLeadGuitar:
                m_result.InstrumentDifficulties.GHLLeadGuitar = ValueParser.Parse<sbyte>(value, "GHL lead guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLRhythmGuitar:
                m_result.InstrumentDifficulties.GHLRhythmGuitar = ValueParser.Parse<sbyte>(value, "GHL rhythm guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLCoopGuitar:
                m_result.InstrumentDifficulties.GHLCoopGuitar = ValueParser.Parse<sbyte>(value, "GHL coop guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLBass:
                m_result.InstrumentDifficulties.GHLBass = ValueParser.Parse<sbyte>(value, "GHL bass difficulty");
                break;
            case IniFormatting.SustainCutoff:
                m_result.Formatting.SustainCutoff = ValueParser.Parse<uint>(value, "sustain cutoff");
                break;
            case IniFormatting.HopoFrequency:
                m_result.Formatting.HopoFrequency = ValueParser.Parse<uint>(value, "hopo frequency");
                break;
            default:
                m_result.UnidentifiedData.Add(new() { Key = entry.Key.ToString(), Value = value.ToString(), Origin = FileType.Ini });
                break;
        }

		void ParseAlbumTrack(in ReadOnlySpan<char> value)
			=> ValueParser.Parse<ushort>(value, "album track");

		void ParseCharter(in ReadOnlySpan<char> value)
		{
			m_result.Charter ??= new();
			m_result.Charter.Name = value.ToString();
		}
	}

	protected override Exception GetHandleException(in ReadOnlyMemory<char> item, Exception innerException)
		=> new SectionException(IniFormatting.Header, GetHandleInnerException(item, innerException));

	public void ApplyToSong(Song song)
	{
		if (song.Metadata is null)
			song.Metadata = Result;
		else
			PropertyMerger.Merge(song.Metadata, false, true, Result);
	}
}
