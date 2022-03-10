using ChartTools.Formatting;
using ChartTools.Internal;
using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Ini
{
    internal abstract class IniParser : FileParser<string>
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result;

        public IniParser(ReadingSession session, Metadata? existing = null) : base(session) => result = existing ??= new();

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
                    result.PlaylistTrack = ValueParser.Parse<ushort>(entry.Value, "playlist track", ushort.TryParse);
                    break;
                case IniFormatting.Year:
                    result.Year = ValueParser.Parse<ushort>(entry.Value, "year", ushort.TryParse);
                    break;
                case IniFormatting.Genre:
                    result.Genre = entry.Value;
                    break;
                case IniFormatting.Charter:
                    ParseCharter(entry.Value);
                    result.Formatting ??= new();
                    result.Formatting.CharterKey |= CharterKey.Charter;
                    break;
                case IniFormatting.Frets:
                    ParseCharter(entry.Value);
                    result.Formatting ??= new();
                    result.Formatting.CharterKey |= CharterKey.Frets;
                    break;
                case IniFormatting.Icon:
                    result.Charter ??= new();
                    result.Charter.Icon = entry.Value;
                    break;
                case IniFormatting.PreviewStart:
                    result.PreviewStart = ValueParser.Parse<uint>(entry.Value, "preview start", uint.TryParse);
                    break;
                case IniFormatting.PreviewEnd:
                    result.PreviewEnd = ValueParser.Parse<uint>(entry.Value, "preview end", uint.TryParse);
                    break;
                case IniFormatting.AudioOffset:
                    result.AudioOffset = ValueParser.Parse<int>(entry.Value, "audio offset", int.TryParse);
                    break;
                case IniFormatting.VideoOffset:
                    result.VideoOffset = ValueParser.Parse<int>(entry.Value, "video offset", int.TryParse);
                    break;
                case IniFormatting.Length:
                    result.Length = ValueParser.Parse<uint>(entry.Value, "song length", uint.TryParse);
                    break;
                case IniFormatting.Difficulty:
                    result.Difficulty = ValueParser.Parse<sbyte>(entry.Value, "difficulty", sbyte.TryParse);
                    break;
                case IniFormatting.LoadingText:
                    result.LoadingText = entry.Value;
                    break;
                case IniFormatting.Modchart:
                    result.IsModchart = ValueParser.Parse<bool>(entry.Value, "modchart", bool.TryParse);
                    break;
                case IniFormatting.SustainCutoff:
                    result.Formatting ??= new();
                    result.Formatting.SustainCutoff = ValueParser.Parse<uint>(entry.Value, "sustain cutoff", uint.TryParse);
                    break;
                case IniFormatting.HopoFrequency:
                    result.Formatting ??= new();
                    result.Formatting.HopoFrequency = ValueParser.Parse<uint>(entry.Value, "hopo frequency", uint.TryParse);
                    break;
                case IniFormatting.HopoFrequencyStep:
                    result.Formatting ??= new();
                    result.Formatting.HopoFrequencyStep = (HopoFrequencyStep)ValueParser.Parse<byte>(entry.Value, "hopo frequency step", byte.TryParse);
                    break;
                case IniFormatting.ForceEightHopoFrequency:
                    result.Formatting ??= new();
                    result.Formatting.ForceEightHopoFrequency = ValueParser.Parse<bool>(entry.Value, "force eight hopo frequency", bool.TryParse);
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Ini });
                    break;
            }

            void ParsealbumTrack(string value) => result.AlbumTrack = ValueParser.Parse<ushort>(value, "album track", ushort.TryParse);
            void ParseCharter(string value)
            {
                result.Charter ??= new();
                result.Charter.Name = entry.Value;
            }
        }

        public override void ApplyResultToSong(Song song)
        {
            if (song.Metadata is null)
                song.Metadata = Result;
            else
                PropertyMerger.Merge(song.Metadata, false, Result);
        }
    }
}
