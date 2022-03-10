using ChartTools.Internal;
using ChartTools.IO.Configuration.Sessions;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class MetadataParser : ChartParser
    {
        public override Metadata Result => GetResult(result);
        private readonly Metadata result = new();

        public MetadataParser(ReadingSession session, Metadata? existing = null) : base(session) => result = existing ?? new();

        protected override void HandleItem(string line)
        {
            TextEntry entry = new(line);
            var value = entry.Value.Trim('"');

            switch (entry.Key)
            {
                case "Name":
                    result.Title = value;
                    break;
                case "Artist":
                    result.Artist = value;
                    break;
                case "Charter":
                    result.Charter = new() { Name = value };
                    break;
                case "Album":
                    result.Album = value;
                    break;
                case "Year":
                    result.Year = ValueParser.ParseUshort(value.TrimStart(','), "year");
                    break;
                case "Offset":
                    result.AudioOffset = (int)(ValueParser.ParseFloat(value, "audio offset") * 1000);
                    break;
                case "Difficulty":
                    result.Difficulty = ValueParser.ParseSbyte(value, "difficulty");
                    break;
                case "PreviewStart":
                    result.PreviewStart = ValueParser.ParseUint(value, "preview start");
                    break;
                case "PreviewEnd":
                    result.PreviewEnd = ValueParser.ParseUint(value, "preview end");
                    break;
                case "Genre":
                    result.Genre = value;
                    break;
                case "MediaType":
                    result.MediaType = value;
                    break;
                case "MusicStream":
                    result.Streams.Music = value;
                    break;
                case "GuitarStream":
                    result.Streams.Guitar = value;
                    break;
                case "BassStream":
                    result.Streams.Bass = value;
                    break;
                case "RhythmStream":
                    result.Streams ??= new() { Rhythm = value };
                    break;
                case "KeysStream":
                    result.Streams ??= new() { Keys = value };
                    break;
                case "DrumStream":
                    result.Streams ??= new() { Drum = value };
                    break;
                case "Drum2Stream":
                    result.Streams ??= new() { Drum2 = value };
                    break;
                case "Drum3Stream":
                    result.Streams ??= new() { Drum3 = value };
                    break;
                case "Drum4Stream":
                    result.Streams ??= new() { Drum4 = value };
                    break;
                case "VocalStream":
                    result.Streams ??= new() { Vocal = value };
                    break;
                case "CrowdStream":
                    result.Streams ??= new() { Crowd = value };
                    break;
                default:
                    result.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Chart });
                    break;
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
