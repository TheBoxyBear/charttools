using ChartTools.Formatting;
using ChartTools.Internal;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Ini.Parsers
{
    internal class MetadataParser : IniParser
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result;

        public MetadataParser(ReadingSession session, Metadata? existing = null) : base(session) => result = existing ?? new();

        protected override void HandleItem(string item)
        {
            var entry = new TextEntry(item);

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
                    ParsealbumTrack(entry.Value);
                    result.Formatting ??= new();
                    result.Formatting.AlbumTrackKey |= AlbumTrackKey.AlbumTrack;
                    break;
                case IniFormatting.Track:
                    ParsealbumTrack(entry.Value);
                    result.Formatting ??= new();
                    result.Formatting.AlbumTrackKey |= AlbumTrackKey.Track;
                    break;
                case IniFormatting.Playlist:
                    result.Playlist = entry.Value;
                    break;
                case IniFormatting.SubPlaylist:
                    result.SubPlaylist = entry.Value;
                    break;
                case IniFormatting.PlaylistTrack:
                    result.PlaylistTrack = ValueParser.ParseUshort(entry.Value, "playlist track");
                    break;
                case IniFormatting.Year:
                    result.Year = ValueParser.ParseUshort(entry.Value, "year");
                    break;
                case IniFormatting.Genre:
                    result.Genre = entry.Value;
                    break;
                case IniFormatting.Charter:
                    ParseCharter();
                    result.Formatting ??= new();
                    result.Formatting.CharterKey |= CharterKey.Charter;
                    break;
                case IniFormatting.Frets:
                    ParseCharter();
                    result.Formatting ??= new();
                    result.Formatting.CharterKey |= CharterKey.Frets;
                    break;
                case IniFormatting.Icon:
                    result.Charter ??= new();
                    result.Charter.Icon = entry.Value;
                    break;
                case IniFormatting.PreviewStart:
                    result.PreviewStart = ValueParser.ParseUint(entry.Value, "preview start");
                    break;
                case IniFormatting.PreviewEnd:
                    result.PreviewEnd = ValueParser.ParseUint(entry.Value, "preview end");
                    break;
                case IniFormatting.AudioOffset:
                    result.AudioOffset = ValueParser.ParseInt(entry.Value, "audio offset");
                    break;
                case IniFormatting.VideoOffset:
                    result.VideoOffset = ValueParser.ParseInt(entry.Value, "video offset");
                    break;
                case IniFormatting.Length:
                    result.Length = ValueParser.ParseUint(entry.Value, "song length");
                    break;
                case IniFormatting.Difficulty:
                    result.Difficulty = ValueParser.ParseSbyte(entry.Value, "difficulty");
                    break;
                case IniFormatting.LoadingText:
                    result.LoadingText = entry.Value;
                    break;
                case IniFormatting.Modchart:
                    result.IsModchart = ValueParser.ParseBool(entry.Value, "modchart");
                    break;
                case IniFormatting.SustainCutoff:
                    result.Formatting ??= new();
                    result.Formatting.SustainCutoff = ValueParser.ParseUint(entry.Value, "sustain cutoff");
                    break;
                case IniFormatting.HopoFrequency:
                    result.Formatting ??= new();
                    result.Formatting.HopoFrequency = ValueParser.ParseUint(entry.Value, "hopo frequency");
                    break;
                case IniFormatting.HopoFrequencyStep:
                    result.Formatting ??= new();
                    result.Formatting.HopoFrequencyStep = (HopoFrequencyStep)ValueParser.ParseByte(entry.Value, "hopo frequency step");
                    break;
                case IniFormatting.ForceEightHopoFrequency:
                    result.Formatting ??= new();
                    result.Formatting.ForceEightHopoFrequency = ValueParser.ParseBool(entry.Value, "force eight hopo frequency");
                    break;
                default:
                    if (!IniFormatting.DifficultyKeys.ContainsKey(entry.Key))
                        result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Ini });
                    break;
            }

            void ParsealbumTrack(string value) => result.AlbumTrack = ValueParser.ParseUshort(value, "album track");
            void ParseCharter()
            {
                result.Charter ??= new();
                result.Charter.Name = entry.Value;
            }
        }

        public override void ApplyToSong(Song song)
        {
            if (song.Metadata is null)
                song.Metadata = Result;
            else
                PropertyMerger.Merge(song.Metadata, false, Result);
        }
    }
}
