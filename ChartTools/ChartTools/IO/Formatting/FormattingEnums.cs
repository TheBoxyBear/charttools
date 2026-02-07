namespace ChartTools.IO.Formatting;

/// <summary>
/// Key used to serialize <see cref="Meta.AlbumTrack"/>
/// </summary>
[Flags]
public enum AlbumTrackKey : byte
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
/// Key used to serialize <see cref="Charter.Name"/>
/// </summary>
[Flags]
public enum CharterKey : byte
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

