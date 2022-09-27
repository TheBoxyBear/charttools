using System;

namespace ChartTools.IO.Chart.Parsing
{
    internal class MetadataParser : ChartParser
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result = new();

        public MetadataParser(Metadata? existing = null) : base(null!, ChartFormatting.MetadataHeader) => result = existing ?? new();

        protected override void HandleItem(string line)
        {
            TextEntry entry = new(line);
            var value = entry.Value.Trim('"');

            switch (entry.Key)
            {
                case ChartFormatting.Title:
                    result.Title = value;
                    break;
                case ChartFormatting.Artist:
                    result.Artist = value;
                    break;
                case ChartFormatting.Charter:
                    result.Charter.Name = value;
                    break;
                case ChartFormatting.Album:
                    result.Album = value;
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
                    result.Genre = value;
                    break;
                case ChartFormatting.MediaType:
                    result.MediaType = value;
                    break;
                case ChartFormatting.MusicStream:
                    result.Streams.Music = value;
                    break;
                case ChartFormatting.GuitarStream:
                    result.Streams.Guitar = value;
                    break;
                case ChartFormatting.BassStream:
                    result.Streams.Bass = value;
                    break;
                case ChartFormatting.RhythmStream:
                    result.Streams.Rhythm = value;
                    break;
                case ChartFormatting.KeysStream:
                    result.Streams.Keys = value;
                    break;
                case ChartFormatting.DrumStream:
                    result.Streams.Drum = value;
                    break;
                case ChartFormatting.Drum2Stream:
                    result.Streams.Drum2 = value;
                    break;
                case ChartFormatting.Drum3Stream:
                    result.Streams.Drum3 = value;
                    break;
                case ChartFormatting.Drum4Stream:
                    result.Streams.Drum4 = value;
                    break;
                case ChartFormatting.VocalStream:
                    result.Streams.Vocals = value;
                    break;
                case ChartFormatting.CrowdStream:
                    result.Streams.Crowd = value;
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileType.Chart });
                    break;
            }
        }

        public override void ApplyToSong(Song song) => song.Metadata = Result;
    }
}
