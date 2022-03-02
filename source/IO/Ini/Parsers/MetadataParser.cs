using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Ini.Parsers
{
    internal class MetadataParser : IniParser
    {
        private readonly Metadata result = new();
        public override Metadata Result => GetResult(result);

        public MetadataParser(ReadingSession session) : base(session) { }

        public override void ApplyResultToSong(Song song) => song.Metadata = Result;

        protected override void HandleItem(string item)
        {
            var entry = new TextEntry(item);

            switch (entry.Key)
            {
                case IniFormatting.MetadataTitle:
                    result.Title = entry.Value;
                    break;
                case IniFormatting.MetadataArtist:
                    result.Artist = entry.Value;
                    break;
                case IniFormatting.MetadataAlbum:
                    result.Album = entry.Value;
                    break;
                case "album_track" or "track":
                    result.AlbumTrack = ushort.TryParse(entry.Value, out ushort ushortValue) ? ushortValue
                        : throw new FormatException($"Cannot parse album track \"{entry.Value}\" to ushort.");
                    break;
                case "playlist_track":
                    result.PlaylistTrack = ushort.TryParse(entry.Value, out ushortValue) ? ushortValue
                        : throw new FormatException($"Cannot parse playlist track \"{entry.Value}\" to ushort.");
                    break;
                case "year":
                    result.Year = ushort.TryParse(entry.Value, out ushortValue) ? ushortValue
                        : throw new FormatException($"Cannot parse year \"{entry.Value}\" to ushort.");
                    break;
                case "genre":
                    result.Genre = entry.Value;
                    break;
                case "charter" or "frets":
                    result.Charter ??= new Charter();
                    result.Charter.Name = entry.Value;
                    break;
                case "icon":
                    result.Charter ??= new();
                    result.Charter.Icon = entry.Value;
                    break;
                case "preview_start_time":
                    result.PreviewStart = uint.TryParse(entry.Value, out uint uintValue) ? uintValue
                        : throw new FormatException($"Cannot parse preview start \"{entry.Value}\" to uint.");
                    break;
                case "preview_end_time":
                    result.PreviewEnd = uint.TryParse(entry.Value, out uintValue) ? uintValue
                        : throw new FormatException($"Cannot parse preview end \"{entry.Value}\" to uint.");
                    break;
                case "delay":
                    result.AudioOffset = int.TryParse(entry.Value, out int intValue) ? intValue
                        : throw new FormatException($"Cannot parse audio offset \"{entry.Value}\" to int.");
                    break;
                case "video_start_time":
                    result.VideoOffset = int.TryParse(entry.Value, out intValue) ? intValue
                        : throw new FormatException($"Cannot parse video offset \"{entry.Value}\" to int.");
                    break;
                case "song_length":
                    result.Length = uint.TryParse(entry.Value, out uintValue) ? uintValue
                        : throw new FormatException($"Cannot parse song length \"{entry.Value}\" to uint.");
                    break;
                case "loading_text":
                    result.LoadingText = entry.Value;
                    break;
                case "modchart":
                    result.IsModchart = bool.TryParse(entry.Value, out bool boolValue) ? boolValue
                        : throw new FormatException($"Cannot parse modchart \"{entry.Value}\" to bool.");
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Ini });
                    break;
            }
        }
    }
}
