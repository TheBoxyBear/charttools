using ChartTools.IO.Formatting;
using ChartTools.IO.Parsing;
using ChartTools.Tools;

namespace ChartTools.IO.Ini;

internal class IniParser(Metadata? existing = null) : TextParser(IniFormatting.Header), ISongAppliable
{
	public override Metadata Result => GetResult(result);
	private readonly Metadata result = existing ?? new();

	protected override void HandleItem(string item)
	{
		TextEntry entry = new(item);

		if (entry.Value is null)
			return;

		switch (entry.Key)
		{
			case IniFormatting.Title:
				result.Title = entry.Value;
				break;
			case IniFormatting.Artist:
				result.Artist = entry.Value;
				break;
			case IniFormatting.Album:
				result.Album = entry.Value;
				break;
			case IniFormatting.AlbumTrack:
				ParseAlbumTrack();
				result.Formatting.AlbumTrackKey |= AlbumTrackKey.AlbumTrack;
				break;
			case IniFormatting.Track:
				ParseAlbumTrack();
				result.Formatting.AlbumTrackKey |= AlbumTrackKey.Track;
				break;
			case IniFormatting.Playlist:
				result.Playlist = entry.Value;
				break;
			case IniFormatting.SubPlaylist:
				result.SubPlaylist = entry.Value;
				break;
			case IniFormatting.PlaylistTrack:
				result.PlaylistTrack = ValueParser.Parse<ushort>(entry.Value, "playlist track");
				break;
			case IniFormatting.Year:
				result.Year = ValueParser.Parse<ushort>(entry.Value, "year");
				break;
			case IniFormatting.Genre:
				result.Genre = entry.Value;
				break;
			case IniFormatting.Charter:
				ParseCharter();
				result.Formatting.CharterKey |= CharterKey.Charter;
				break;
			case IniFormatting.Frets:
				ParseCharter();
				result.Formatting.CharterKey |= CharterKey.Frets;
				break;
			case IniFormatting.Icon:
				result.Charter.Icon = entry.Value;
				break;
			case IniFormatting.PreviewStart:
				result.PreviewStart = entry.Value.StartsWith('-') ? null : ValueParser.Parse<uint>(entry.Value, "preview start");
				break;
			case IniFormatting.PreviewEnd:
				result.PreviewEnd = entry.Value.StartsWith('-') ? null : ValueParser.Parse<uint>(entry.Value, "preview end");
				break;
			case IniFormatting.AudioOffset:
				result.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<int>(entry.Value, "audio offset"));
				break;
			case IniFormatting.VideoOffset:
				result.VideoOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<int>(entry.Value, "video offset"));
				break;
			case IniFormatting.Length:
				result.Length = ValueParser.Parse<uint>(entry.Value, "song length");
				break;
			case IniFormatting.LoadingText:
				result.LoadingText = entry.Value;
				break;
			case IniFormatting.Modchart:
				result.IsModchart = ValueParser.Parse<int>(entry.Value, "modchart") == 1;
				break;
			case IniFormatting.Explicit:
				result.Explicit = ValueParser.Parse<bool>(entry.Value, "explicit");
				break;
			case IniFormatting.Difficulties.Global:
				result.Difficulty = ValueParser.Parse<sbyte>(entry.Value, "difficulty");
				break;
			case IniFormatting.Difficulties.StandardLeadGuitar:
				result.InstrumentDifficulties.StandardLeadGuitar = ValueParser.Parse<sbyte>(entry.Value, "lead guitar difficulty");
				break;
			case IniFormatting.Difficulties.StandardRhythmGuitar:
				result.InstrumentDifficulties.StandardRhythmGuitar = ValueParser.Parse<sbyte>(entry.Value, "rhythm guitar difficulty");
				break;
            case IniFormatting.Difficulties.StandardCoopGuitar:
                result.InstrumentDifficulties.StandardCoopGuitar = ValueParser.Parse<sbyte>(entry.Value, "coop guitar difficulty");
                break;
            case IniFormatting.Difficulties.StandardBass:
				result.InstrumentDifficulties.StandardBass = ValueParser.Parse<sbyte>(entry.Value, "bass difficulty");
				break;
			case IniFormatting.Difficulties.Drums:
				result.InstrumentDifficulties.Drums = ValueParser.Parse<sbyte>(entry.Value, "drums difficulty");
				break;
			case IniFormatting.Difficulties.StandardKeys:
				result.InstrumentDifficulties.StandardKeys = ValueParser.Parse<sbyte>(entry.Value, "keys difficulty");
				break;
            case IniFormatting.Difficulties.GHLLeadGuitar:
                result.InstrumentDifficulties.GHLLeadGuitar = ValueParser.Parse<sbyte>(entry.Value, "GHL lead guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLRhythmGuitar:
                result.InstrumentDifficulties.GHLRhythmGuitar = ValueParser.Parse<sbyte>(entry.Value, "GHL rhythm guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLCoopGuitar:
                result.InstrumentDifficulties.GHLCoopGuitar = ValueParser.Parse<sbyte>(entry.Value, "GHL coop guitar difficulty");
                break;
            case IniFormatting.Difficulties.GHLBass:
                result.InstrumentDifficulties.GHLBass = ValueParser.Parse<sbyte>(entry.Value, "GHL bass difficulty");
                break;
            case IniFormatting.SustainCutoff:
                result.Formatting.SustainCutoff = ValueParser.Parse<uint>(entry.Value, "sustain cutoff");
                break;
            case IniFormatting.HopoFrequency:
                result.Formatting.HopoFrequency = ValueParser.Parse<uint>(entry.Value, "hopo frequency");
                break;
            default:
                if (entry.Value is not null)
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileType.Ini });
                break;
        }

		void ParseAlbumTrack() => ValueParser.Parse<ushort>(entry.Value, "album track");

		void ParseCharter()
		{
			result.Charter ??= new();
			result.Charter.Name = entry.Value;
		}
	}

	public void ApplyToSong(Song song)
	{
		if (song.Metadata is null)
			song.Metadata = Result;
		else
			PropertyMerger.Merge(song.Metadata, false, true, Result);
	}
}
