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
        SysExProSlide = "sysex_pro_slide",
        GuitarDifficulty = "diff_guitar",
        BassDifficulty = "diff_bass",
        DrumsDifficulty = "diff_drums",
        KeysDifficulty = "diff_keys",
        GHLGuitarDifficulty = "diff_guitarghl",
        GHLBassDifficulty = "diff_bassghl";

    public static string Line(string key, string? value) => $"{key} = {value}";
}
