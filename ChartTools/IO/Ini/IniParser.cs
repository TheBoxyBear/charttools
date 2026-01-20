using ChartTools.IO.Formatting;
using ChartTools.IO.Parsing;
using ChartTools.Meta;
using ChartTools.Tools;

namespace ChartTools.IO.Ini;

internal class IniParser(Metadata? existing = null)
	: TextParser(IniFormatting.Header.AsMemory()), ISongAppliable
{
	public override Metadata Result => GetResult(result);
	private readonly Metadata result = existing ?? new();

	protected override void HandleItem(in ReadOnlyMemory<char> item)
	{
		TextEntry entry = new(item);
		ReadOnlySpan<char> value  = entry.Value.Span;

		switch (entry.Key.Span)
		{
			case IniFormatting.Title:
				result.Title = value.ToString();
				break;
			case IniFormatting.Artist:
				result.Artist = value.ToString();
				break;
			case IniFormatting.Album:
				result.Album = value.ToString();
				break;
			case IniFormatting.AlbumTrack:
				ParseAlbumTrack(in value);
				result.Formatting.AlbumTrackKey |= AlbumTrackKey.AlbumTrack;
				break;
			case IniFormatting.Track:
				ParseAlbumTrack(in value);
				result.Formatting.AlbumTrackKey |= AlbumTrackKey.Track;
				break;
			case IniFormatting.Playlist:
				result.Playlist = value.ToString();
				break;
			case IniFormatting.SubPlaylist:
				result.SubPlaylist = value.ToString();
				break;
			case IniFormatting.PlaylistTrack:
				result.PlaylistTrack = ValueParser.Parse<ushort>(value, "playlist track");
				break;
			case IniFormatting.Year:
				result.Year = ValueParser.Parse<ushort>(value, "year");
				break;
			case IniFormatting.Genre:
				result.Genre = value.ToString();
				break;
			case IniFormatting.Charter:
				ParseCharter(in value);
				result.Formatting.CharterKey |= CharterKey.Charter;
				break;
			case IniFormatting.Frets:
				ParseCharter(in value);
				result.Formatting.CharterKey |= CharterKey.Frets;
				break;
			case IniFormatting.Icon:
				result.Charter.Icon = value.ToString();
				break;
			case IniFormatting.PreviewStart:
				result.PreviewStart = value.StartsWith('-') ? null : ValueParser.Parse<uint>(value, "preview start");
				break;
			case IniFormatting.PreviewEnd:
				result.PreviewEnd = value.StartsWith('-') ? null : ValueParser.Parse<uint>(value, "preview end");
				break;
			case IniFormatting.AudioOffset:
				result.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<int>(value, "audio offset"));
				break;
			case IniFormatting.VideoOffset:
				result.VideoOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<int>(value, "video offset"));
				break;
			case IniFormatting.Length:
				result.Length = ValueParser.Parse<uint>(value, "song length");
				break;
			case IniFormatting.LoadingText:
				result.LoadingText = value.ToString();
				break;
			case IniFormatting.Modchart:
				result.IsModchart = ValueParser.Parse<int>(value, "modchart") == 1;
				break;
			case IniFormatting.Explicit:
				result.Explicit = ValueParser.Parse<bool>(value, "explicit");
				break;
			case IniFormatting.Difficulties.Global:
				result.Difficulty = ValueParser.Parse<sbyte>(value, "difficulty");
				break;
			case IniFormatting.Difficulties.StandardLeadGuitar:
				result.InstrumentDifficulties.StandardLeadGuitar = ValueParser.Parse<sbyte>(value, "lead guitar difficulty");
				break;
			case IniFormatting.Difficulties.StandardRhythmGuitar:
				result.InstrumentDifficulties.StandardRhythmGuitar = ValueParser.Parse<sbyte>(value, "rhythm guitar difficulty");
				break;
            case IniFormatting.Difficulties.StandardCoopGuitar:
                result.InstrumentDifficulties.StandardCoopGuitar = ValueParser.Parse<sbyte>(value, "coop guitar difficulty");
                break;
            case IniFormatting.Difficulties.StandardBass:
				result.InstrumentDifficulties.StandardBass = ValueParser.Parse<sbyte>(value, "bass difficulty");
				break;
			case IniFormatting.Difficulties.Drums:
				result.InstrumentDifficulties.Drums = ValueParser.Parse<sbyte>(value, "drums difficulty");
				break;
			case IniFormatting.Difficulties.StandardKeys:
				result.InstrumentDifficulties.StandardKeys = ValueParser.Parse<sbyte>(value, "keys difficulty");
				break;
            case IniFormatting.Difficulties.GHLLeadGuitar:
                result.InstrumentDifficulties.GHLLeadGuitar = ValueParser.Parse<sbyte>(value, "GHL lead guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLRhythmGuitar:
                result.InstrumentDifficulties.GHLRhythmGuitar = ValueParser.Parse<sbyte>(value, "GHL rhythm guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLCoopGuitar:
                result.InstrumentDifficulties.GHLCoopGuitar = ValueParser.Parse<sbyte>(value, "GHL coop guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLBass:
                result.InstrumentDifficulties.GHLBass = ValueParser.Parse<sbyte>(value, "GHL bass difficulty");
                break;
            case IniFormatting.SustainCutoff:
                result.Formatting.SustainCutoff = ValueParser.Parse<uint>(value, "sustain cutoff");
                break;
            case IniFormatting.HopoFrequency:
                result.Formatting.HopoFrequency = ValueParser.Parse<uint>(value, "hopo frequency");
                break;
            default:
                result.UnidentifiedData.Add(new()
				{
					Key    = entry.Key.ToString(),
					Value  = value.ToString(),
					Origin = FileType.Ini });
                break;
        }

		void ParseAlbumTrack(in ReadOnlySpan<char> value)
			=> ValueParser.Parse<ushort>(value, "album track");

		void ParseCharter(in ReadOnlySpan<char> name)
		{
			result.Charter ??= new();
			result.Charter.Name = name.ToString();
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
