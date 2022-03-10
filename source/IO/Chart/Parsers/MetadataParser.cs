using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class MetadataParser : ChartParser
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result = new();

        public MetadataParser(ReadingSession session) : base(session) { }

        protected override void HandleItem(string line)
        {
            ChartEntry entry = new(line);

            string data = entry.Data.Trim('"');

            switch (entry.Header)
            {
                case "Name":
                    result.Title = data;
                    break;
                case "Artist":
                    result.Artist = data;
                    break;
                case "Charter":
                    result.Charter = new() { Name = data };
                    break;
                case "Album":
                    result.Album = data;
                    break;
                case "Year":
                    result.Year = ValueParser.ParseUshort(data.TrimStart(','), "year");
                    break;
                case "Offset":
                    result.AudioOffset = (int)(ValueParser.ParseFloat(data, "audio offset") * 1000);
                    break;
                case "Resolution":
                    result.Resolution = ValueParser.ParseUshort(data, "resolution");
                    break;
                case "Difficulty":
                    result.Difficulty = ValueParser.ParseSbyte(data, "difficulty");
                    break;
                case "PreviewStart":
                    result.PreviewStart = ValueParser.ParseUint(data, "preview start");
                    break;
                case "PreviewEnd":
                    result.PreviewEnd = ValueParser.ParseUint(data, "preview end");
                    break;
                case "Genre":
                    result.Genre = data;
                    break;
                case "MediaType":
                    result.MediaType = data;
                    break;
                case "MusicStream":
                    result.Streams.Music = data;
                    break;
                case "GuitarStream":
                    result.Streams.Guitar = data;
                    break;
                case "BassStream":
                    result.Streams.Bass = data;
                    break;
                case "RhythmStream":
                    result.Streams ??= new();
                    result.Streams.Rhythm = data;
                    break;
                case "KeysStream":
                    result.Streams ??= new();
                    result.Streams.Keys = data;
                    break;
                case "DrumStream":
                    result.Streams ??= new();
                    result.Streams.Drum = data;
                    break;
                case "Drum2Stream":
                    result.Streams ??= new();
                    result.Streams.Drum2 = data;
                    break;
                case "Drum3Stream":
                    result.Streams ??= new();
                    result.Streams.Drum3 = data;
                    break;
                case "Drum4Stream":
                    result.Streams ??= new();
                    result.Streams.Drum4 = data;
                    break;
                case "VocalStream":
                    result.Streams ??= new();
                    result.Streams.Vocal = data;
                    break;
                case "CrowdStream":
                    result.Streams ??= new();
                    result.Streams.Crowd = data;
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Header, Data = entry.Data, Origin = FileFormat.Chart });
                    break;
            }
        }

        public override void ApplyResultToSong(Song song) => song.Metadata = result;
    }
}
