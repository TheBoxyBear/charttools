namespace ChartTools.IO.Formatting;

/// <summary>
/// Keys used to serialize <see cref="Meta.Metadata.AlbumTrack"/>
/// </summary>
[Flags]
public enum AlbumTrackKeys : byte
{
	Default = 0,
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.AlbumTrack"/>
    /// </summary>
    AlbumTrack = 1 << 0,
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.Track"/>
    /// </summary>
    Track = 1 << 1
}

/// <summary>
/// Keys used to serialize <see cref="Meta.Charter.Name"/>
/// </summary>
[Flags]
public enum CharterKeys : byte
{
	Default = 0,
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.Charter"/>
    /// </summary>
    Charter = 1 << 0,
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.Frets"/>
    /// </summary>
    Frets = 1 << 1
}
