using System.Collections.Generic;

namespace ChartTools.IO.Ini
{
    internal static class IniFormatting
    {
        /// <summary>
        /// Keys for <see cref="Instrument"/> difficulties
        /// </summary>
        public static readonly Dictionary<string, InstrumentIdentity> DifficultyKeys = new()
        {
            { "diff_drums", InstrumentIdentity.Drums },
            { "diff_guitarghl", InstrumentIdentity.GHLGuitar },
            { "diff_bassghl", InstrumentIdentity.GHLBass },
            { "diff_guitar", InstrumentIdentity.LeadGuitar },
            { "diff_bass", InstrumentIdentity.Bass },
            { "diff_keys", InstrumentIdentity.Keys }
        };

        public const string
            MetadataTitle = "name",
            MetadataArtist = "artist",
            MetadataAlbum = "album",
            MetadataAlbumTrack = "album_track",
            MetadataPlaylistTrack = "playlis_track",
            MetadataGenre = "genre",
            MetadataYear = "year",
            MetadataPreviewStart = "preview_start_time",
            MetadataPreviewEnd = "preview_end_time",
            MetadataAudioOffset = "delay",
            MetadataVideoOffset = "video_start_time",
            MetadataLength = "song_length",
            MetadataLoadingText = "loading_text",
            MetadataModchart = "modchart";

        public static readonly Dictionary<string, string> MetadataKeys = new()
        {
            { nameof(Metadata.Title), MetadataTitle },
            { nameof(Metadata.Artist), MetadataArtist },
            { nameof(Metadata.Album), MetadataAlbum },
            { nameof(Metadata.AlbumTrack), MetadataAlbumTrack },
            { nameof(Metadata.PlaylistTrack), MetadataGenre },
            { nameof(Metadata.Year), MetadataYear },
            { nameof(Metadata.PreviewStart), MetadataPreviewStart },
            { nameof(Metadata.PreviewEnd), MetadataPreviewEnd },
            { nameof(Metadata.AudioOffset), MetadataAudioOffset },
            { nameof(Metadata.VideoOffset), MetadataVideoOffset },
            { nameof(Metadata.Length), MetadataLength },
            { nameof(Metadata.LoadingText), MetadataLoadingText },
            { nameof(Metadata.IsModchart), MetadataModchart }
        };

        public static string Entry(string key, string value) => $"{key} = {value}";
    }
}
