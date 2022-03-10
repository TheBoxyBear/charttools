using System.Collections.Generic;

namespace ChartTools.IO.Ini
{
    public static class IniFormatting
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
            Title = "name",
            Artist = "artist",
            Album = "album",
            AlbumTrack = "album_track",
            Track = "track",
            Playlist = "playlist",
            SubPlaylist = "sub_playlist",
            PlaylistTrack = "playlis_track",
            Genre = "genre",
            Year = "year",
            Charter = "charter",
            Frets = "frets",
            Icon = "icon",
            PreviewStart = "preview_start_time",
            PreviewEnd = "preview_end_time",
            AudioOffset = "delay",
            VideoOffset = "video_start_time",
            Length = "song_length",
            LoadingText = "loading_text",
            Difficulty = "diff_band",
            Modchart = "modchart",
            SustainCutoff = "sustain_cutoff_threshold",
            HopoFrequency = "hopo_frequency",
            HopoFrequencyStep = "hopofreq",
            ForceEightHopoFrequency = "eighthnote_hopo",
            MultiplierNote = "multiplier_note",
            StarPowerNote = "star_power_note",
            SysExSliders = "sysex_slider",
            SysExHighHat = "sysex_high_hat_ctrl",
            Rimshot = "sysex_rimshot",
            SysExOpenBass = "sysex_open_bass",
            SysExProSlide = "sysex_pro_slide";

        public static string Line(string key, string value) => $"{key} = {value}";
    }
}
