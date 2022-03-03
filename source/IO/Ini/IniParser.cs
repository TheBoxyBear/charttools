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
                case IniFormatting.PlaylistTrack:
                    result.PlaylistTrack = ushort.TryParse(entry.Value, out ushort ushortValue) ? ushortValue
                        : throw IOExceptions.Parse("playlist track", entry.Value, "ushort");
                    break;
                case IniFormatting.Year:
                    result.Year = ushort.TryParse(entry.Value, out ushortValue) ? ushortValue
                        : throw IOExceptions.Parse("year", entry.Value, "ushort");
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
                    result.PreviewStart = uint.TryParse(entry.Value, out uint uintValue) ? uintValue
                        : throw IOExceptions.Parse("preview start", entry.Value, "uint");
                    break;
                case IniFormatting.PreviewEnd:
                    result.PreviewEnd = uint.TryParse(entry.Value, out uintValue) ? uintValue
                        : throw IOExceptions.Parse("pewview end", entry.Value, "uint");
                    break;
                case IniFormatting.AudioOffset:
                    result.AudioOffset = int.TryParse(entry.Value, out int intValue) ? intValue
                        : throw IOExceptions.Parse("audio offset", entry.Value, "int");
                    break;
                case IniFormatting.VideoOffset:
                    result.VideoOffset = int.TryParse(entry.Value, out intValue) ? intValue
                        : throw IOExceptions.Parse("video offset", entry.Value, "int");
                    break;
                case IniFormatting.Length:
                    result.Length = uint.TryParse(entry.Value, out uintValue) ? uintValue
                        : throw IOExceptions.Parse("song length", entry.Value, "uint");
                    break;
                case IniFormatting.LoadingText:
                    result.LoadingText = entry.Value;
                    break;
                case IniFormatting.Modchart:
                    result.IsModchart = bool.TryParse(entry.Value, out bool boolValue) ? boolValue
                        : throw IOExceptions.Parse("modchart", entry.Value, "bool");
                    break;
                case IniFormatting.SustainCutoff:
                    result.Formatting ??= new();
                    result.Formatting.SustainCutoff = uint.TryParse(entry.Value, out uintValue) ? uintValue
                        : throw IOExceptions.Parse("sustain cutoff", entry.Value, "uint");
                    break;
                case IniFormatting.HopoFrequency:
                    result.Formatting ??= new();
                    result.Formatting.HopoFrequency = uint.TryParse(entry.Value, out uintValue) ? uintValue
                        : throw IOExceptions.Parse("hopo frequency", entry.Value, "uint");
                    break;
                case IniFormatting.HopoFrequencyStep:
                    result.Formatting ??= new();
                    result.Formatting.HopoFrequencyStep = byte.TryParse(entry.Value, out byte byteValue) ? (HopoFrequencyStep)byteValue
                        : throw IOExceptions.Parse("hopo frequency step", entry.Value, "byte");
                    break;
                case IniFormatting.ForceEightHopoFrequency:
                    result.Formatting ??= new();
                    result.Formatting.ForceEightHopoFrequency = bool.TryParse(entry.Value, out boolValue) ? boolValue
                        : throw IOExceptions.Parse("force eight hopo frequency", entry.Value, "bool");
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Ini });
                    break;
            }

            void ParsealbumTrack(string value) => result.AlbumTrack = ushort.TryParse(entry.Value, out ushort ushortValue) ? ushortValue
                       : throw IOExceptions.Parse("album track", entry.Value, "ushort");
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
