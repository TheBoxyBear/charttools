namespace ChartTools.IO.Ini;

public static class IniFormatting
{
    public const string
        Header = "[song]",
        Title = "name",
        Artist = "artist",
        Album = "album",
        AlbumTrack = "album_track",
        Track = "track",
        Playlist = "playlist",
        SubPlaylist = "sub_playlist",
        PlaylistTrack = "playlis_track",
        Genre = "genre",
        Explicit = "explicit_lyrics",
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

    public class Difficulties
    {
        public const string Global = "diff_band";
        public const string StandardLeadGuitar = "diff_guitar";
        public const string StandardRhythmGuitar = "diff_rhythm";
        public const string StandardCoopGuitar = "diff_guitar_coop";
        public const string StandardBass = "diff_bass";
        public const string Drums = "diff_drums";
        public const string StandardKeys = "diff_keys";
        public const string GHLLeadGuitar = "diff_guitarghl";
        public const string GHLRhythmGuitar = "diff_rhythm_ghl";
        public const string GHLCoopGuitar = "diff_guitar_coop_ghl";
        public const string GHLBass = "diff_bassghl";
    }

	public static string Line(string key, string? value) => $"{key} = {value}";
}
