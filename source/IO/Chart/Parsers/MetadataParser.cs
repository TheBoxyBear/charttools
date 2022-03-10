using ChartTools.Internal;
using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class MetadataParser : TextParser
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result = new();

        public MetadataParser(ReadingSession session, Metadata? existing = null) : base(session) => result = existing ?? new();

        protected override void HandleItem(string line)
        {
            TextEntry entry = new(line);

            string data = entry.Value.Trim('"');

            switch (entry.Key)
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
                    result.Streams ??= new() { Rhythm = data };
                    break;
                case "KeysStream":
                    result.Streams ??= new() { Keys = data };
                    break;
                case "DrumStream":
                    result.Streams ??= new() { Drum = data };
                    break;
                case "Drum2Stream":
                    result.Streams ??= new() { Drum2 = data };
                    break;
                case "Drum3Stream":
                    result.Streams ??= new() { Drum3 = data };
                    break;
                case "Drum4Stream":
                    result.Streams ??= new() { Drum4 = data };
                    break;
                case "VocalStream":
                    result.Streams ??= new() { Vocal = data };
                    break;
                case "CrowdStream":
                    result.Streams ??= new() { Crowd = data };
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Chart });
                    break;
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
