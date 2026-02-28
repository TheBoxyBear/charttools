using ChartTools.IO.Ini;
using ChartTools.Meta;

namespace ChartTools.IO.Formatting;

/// <summary>
/// Keys used to serialize <see cref="Metadata.AlbumTrack"/>
/// </summary>
[Flags]
public enum AlbumTrackKeys : byte
{
	None = 0,
    /// <summary>
    /// Use <see cref="IniFormatting.AlbumTrack"/>
    /// </summary>
    AlbumTrack = 1 << 0,
    /// <summary>
    /// Use <see cref="IniFormatting.Track"/>
    /// </summary>
    Track = 1 << 1,
	All = AlbumTrack | Track
}

/// <summary>
/// Keys used to serialize <see cref="Charter.Name"/>
/// </summary>
[Flags]
public enum CharterKeys : byte
{
	None = 0,
    /// <summary>
    /// Use <see cref="IniFormatting.Charter"/>
    /// </summary>
    Charter = 1 << 0,
    /// <summary>
    /// Use <see cref="IniFormatting.Frets"/>
    /// </summary>
    Frets = 1 << 1,
	All = Charter | Frets
}
