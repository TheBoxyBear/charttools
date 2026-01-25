namespace ChartTools.IO.Formatting;

/// <summary>
/// Keys used to serialize <see cref="Meta.Metadata.AlbumTrack"/>
/// </summary>
[Flags]
public enum AlbumTrackKeys : byte
{
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.AlbumTrack"/>
    /// </summary>
    AlbumTrack,
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.Track"/>
    /// </summary>
    Track
}

/// <summary>
/// Keys used to serialize <see cref="Meta.Charter.Name"/>
/// </summary>
[Flags]
public enum CharterKeys : byte
{
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.Charter"/>
    /// </summary>
    Charter,
    /// <summary>
    /// Use <see cref="Ini.IniFormatting.Frets"/>
    /// </summary>
    Frets
}
