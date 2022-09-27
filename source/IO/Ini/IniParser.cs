using ChartTools.IO.Formatting;
using ChartTools.IO.Parsing;
using ChartTools.Tools;

using System;

namespace ChartTools.IO.Ini
{
    internal class IniParser : TextParser, ISongAppliable
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result;

        public IniParser(Metadata? existing = null) : base(null!, IniFormatting.Header) => result = existing ?? new();

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
                    result.Formatting.AlbumTrackKey |= AlbumTrackKey.AlbumTrack;
                    break;
                case IniFormatting.Track:
                    ParsealbumTrack(entry.Value);
                    result.Formatting.AlbumTrackKey |= AlbumTrackKey.Track;
                    break;
                case IniFormatting.Playlist:
                    result.Playlist = entry.Value;
                    break;
                case IniFormatting.SubPlaylist:
                    result.SubPlaylist = entry.Value;
                    break;
                case IniFormatting.PlaylistTrack:
                    result.PlaylistTrack = ParseNullable(ValueParser.ParseUshort, "playlist track");
                    break;
                case IniFormatting.Year:
                    result.Year = ParseNullable(ValueParser.ParseUshort, "year");
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
                    result.PreviewStart = ParseNullable(ValueParser.ParseUint, "preview start");
                    break;
                case IniFormatting.PreviewEnd:
                    result.PreviewEnd = ParseNullable(ValueParser.ParseUint, "preview end");
                    break;
                case IniFormatting.AudioOffset:
                    result.AudioOffset = entry.Value is null ? null : TimeSpan.FromMilliseconds(ValueParser.ParseInt(entry.Value, "audio offset"));
                    break;
                case IniFormatting.VideoOffset:
                    result.VideoOffset = entry.Value is null ? null : TimeSpan.FromMilliseconds(ValueParser.ParseInt(entry.Value, "video offset"));
                    break;
                case IniFormatting.Length:
                    result.Length = ParseNullable(ValueParser.ParseUint, "song length");
                    break;
                case IniFormatting.Difficulty:
                    result.Difficulty = ParseNullable(ValueParser.ParseSbyte, "difficulty");
                    break;
                case IniFormatting.LoadingText:
                    result.LoadingText = entry.Value;
                    break;
                case IniFormatting.Modchart:
                    result.IsModchart = entry.Value is not null && ValueParser.ParseBool(entry.Value, "modchart");
                    break;
                case IniFormatting.GuitarDifficulty:
                    result.InstrumentDifficulties.Guitar = ParseNullable(ValueParser.ParseSbyte, "guitar difficulty");
                    break;
                case IniFormatting.BassDifficulty:
                    result.InstrumentDifficulties.Bass = ParseNullable(ValueParser.ParseSbyte, "bass difficulty");
                    break;
                case IniFormatting.DrumsDifficulty:
                    result.InstrumentDifficulties.Drums = ParseNullable(ValueParser.ParseSbyte, "drums difficulty");
                    break;
                case IniFormatting.KeysDifficulty:
                    result.InstrumentDifficulties.Keys = ParseNullable(ValueParser.ParseSbyte, "keys difficulty");
                    break;
                case IniFormatting.GHLGuitarDifficulty:
                    result.InstrumentDifficulties.GHLGuitar = ParseNullable(ValueParser.ParseSbyte, "GHL guitar difficulty");
                    break;
                case IniFormatting.GHLBassDifficulty:
                    result.InstrumentDifficulties.GHLBass = ParseNullable(ValueParser.ParseSbyte, "GHL bass difficulty");
                    break;
                case IniFormatting.SustainCutoff:
                    result.Formatting.SustainCutoff = ParseNullable(ValueParser.ParseUint, "sustain cutoff");
                    break;
                case IniFormatting.HopoFrequency:
                    result.Formatting.HopoFrequency = ParseNullable(ValueParser.ParseUint, "hopo frequency");
                    break;
                case IniFormatting.HopoFrequencyStep:
                    result.Formatting.HopoFrequencyStep = (HopoFrequencyStep?)ParseNullable(ValueParser.ParseByte, "hopo frequency step");
                    break;
                case IniFormatting.ForceEightHopoFrequency:
                    result.Formatting.ForceEightHopoFrequency = ParseNullable(ValueParser.ParseBool, "force eight hopo frequency");
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Ini });
                    break;
            }

            void ParsealbumTrack(string? value) => ParseNullable(ValueParser.ParseUshort, "album track");
            void ParseCharter()
            {
                result.Charter ??= new();
                result.Charter.Name = entry.Value;
            }
            T? ParseNullable<T>(Func<string, string, T> parse, string target) where T : struct => entry.Value is null ? null : parse(entry.Value, target);
        }

        public void ApplyToSong(Song song)
        {
            if (song.Metadata is null)
                song.Metadata = Result;
            else
                PropertyMerger.Merge(song.Metadata, false, true, Result);
        }
    }
}
