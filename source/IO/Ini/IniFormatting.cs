using ChartTools.IO.Formatting;

using System;
using System.Collections.Generic;
using System.Reflection;

namespace ChartTools.IO.Ini
{
    internal static class IniFormatting
    {
        public static readonly Type MetadataType = typeof(Metadata), FormattingType = typeof(FormattingRules);

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

        public const string
            FormattingSustianCutoff = "sustain_cutoff_threshold",
            FormattingHopoFrequency = "hopo_frequency",
            FormattingHopoFreq = "hopofreq",
            FormattingEightHopo = "eighthnote_hopo",
            FormattingMultiplierNote = "multiplier_note",
            FormattingStarPowerNote = "star_power_note",
            FormattingSysExSlider = "sysex_slider",
            FormattingSysExHighHat = "sysex_high_hat_ctrl",
            FormattingRimshot = "sysex_rimshot",
            FormattingSysExOpenBass = "sysex_open_bass",
            FormattingSysExProSlide = "sysex_pro_slide";

        public static readonly Dictionary<PropertyInfo, string> MetadataKeys = new()
        {
            { MetadataType.GetProperty(nameof(Metadata.Title))!, MetadataTitle },
            { MetadataType.GetProperty(nameof(Metadata.Artist))!, MetadataArtist },
            { MetadataType.GetProperty(nameof(Metadata.Album))!, MetadataAlbum },
            { MetadataType.GetProperty(nameof(Metadata.AlbumTrack))!, MetadataAlbumTrack },
            { MetadataType.GetProperty(nameof(Metadata.Genre))!, MetadataGenre },
            { MetadataType.GetProperty(nameof(Metadata.Year))!, MetadataYear },
            { MetadataType.GetProperty(nameof(Metadata.PreviewStart))!, MetadataPreviewStart },
            { MetadataType.GetProperty(nameof(Metadata.PreviewEnd))!, MetadataPreviewEnd },
            { MetadataType.GetProperty(nameof(Metadata.AudioOffset))!, MetadataAudioOffset },
            { MetadataType.GetProperty(nameof(Metadata.VideoOffset))!, MetadataVideoOffset },
            { MetadataType.GetProperty(nameof(Metadata.Length))!, MetadataLength },
            { MetadataType.GetProperty(nameof(Metadata.LoadingText))!, MetadataLoadingText },
            { MetadataType.GetProperty(nameof(Metadata.IsModchart))!, MetadataModchart }
        };

        public static readonly Dictionary<string, string> FormattingKeys = new();

        public static string Entry(string key, string value) => $"{key} = {value}";
    }
}
