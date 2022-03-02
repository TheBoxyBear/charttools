using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;

using System;

namespace ChartTools.IO.Chart.Parsers
{
    internal class MetadataFormattingParser : ChartParser
    {
        public record ResultGroup(Metadata Metadata, FormattingRules Formatting);

        public MetadataFormattingParser(ReadingSession session) : base(session) { }

        public override ResultGroup Result => GetResult(result);
        private readonly ResultGroup result = new(new(), new());

        protected override void HandleItem(string line)
        {
            TextEntry entry = new(line);

            string data = entry.Value.Trim('"');

            switch (entry.Key)
            {
                case "Name":
                    result.Metadata.Title = data;
                    break;
                case "Artist":
                    result.Metadata.Artist = data;
                    break;
                case "Charter":
                    result.Metadata.Charter = new() { Name = data };
                    break;
                case "Album":
                    result.Metadata.Album = data;
                    break;
                case "Year":
                    try { result.Metadata.Year = ushort.Parse(data.TrimStart(',')); }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
                case "Offset":
                    try { result.Metadata.AudioOffset = (int)(float.Parse(entry.Value) * 1000); }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
                case "Resolution":
                    try { result.Formatting.Resolution = ushort.Parse(data); }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
                case "Difficulty":
                    try { result.Metadata.Difficulty = sbyte.Parse(data); }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
                case "PreviewStart":
                    try { result.Metadata.PreviewStart = uint.Parse(data); }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
                case "PreviewEnd":
                    try { result.Metadata.PreviewEnd = uint.Parse(data); }
                    catch (Exception e) { throw ChartExceptions.Line(line, e); }
                    break;
                case "Genre":
                    result.Metadata.Genre = data;
                    break;
                case "MediaType":
                    result.Metadata.MediaType = data;
                    break;
                case "MusicStream":
                    result.Metadata.Streams.Music = data;
                    break;
                case "GuitarStream":
                    result.Metadata.Streams.Guitar = data;
                    break;
                case "BassStream":
                    result.Metadata.Streams.Bass = data;
                    break;
                case "RhythmStream":
                    result.Metadata.Streams ??= new() { Rhythm = data };
                    break;
                case "KeysStream":
                    result.Metadata.Streams ??= new() { Keys = data };
                    break;
                case "DrumStream":
                    result.Metadata.Streams ??= new() { Drum = data };
                    break;
                case "Drum2Stream":
                    result.Metadata.Streams ??= new() { Drum2 = data };
                    break;
                case "Drum3Stream":
                    result.Metadata.Streams ??= new() { Drum3 = data };
                    break;
                case "Drum4Stream":
                    result.Metadata.Streams ??= new() { Drum4 = data };
                    break;
                case "VocalStream":
                    result.Metadata.Streams ??= new() { Vocal = data };
                    break;
                case "CrowdStream":
                    result.Metadata.Streams ??= new() { Crowd = data };
                    break;
                default:
                    result.Metadata.UnidentifiedData.Add(new() { Key = entry.Key, Value = entry.Value, Origin = FileFormat.Chart });
                    break;
            }
        }

        public override void ApplyResultToSong(Song song)
        {
            var res = Result;

            song.Metadata = res.Metadata;
            song.Formatting = res.Formatting;
        }
    }
}
