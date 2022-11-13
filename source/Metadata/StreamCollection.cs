using ChartTools.IO.Chart;
using ChartTools.IO.Chart.Serialization;

namespace ChartTools;

/// <summary>
/// Set of audio files to play and mute during gameplay
/// </summary>
/// <remarks>Instrument audio may be muted when chords of the respective instrument are missed</remarks>
public class StreamCollection
{
    /// <summary>
    /// Location of the base audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.MusicStream)]
    public string? Music { get; set; }
    /// <summary>
    /// Location of the guitar audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.GuitarStream)]
    public string? Guitar { get; set; }
    /// <summary>
    /// Location of the bass audio
    /// </summary>
    [ChartKeySerializable(ChartFormatting.BassStream)]
    public string? Bass { get; set; }
    /// <summary>
    /// Location of the rhythm guitar audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.RhythmStream)]
    public string? Rhythm { get; set; }
    /// <summary>
    /// Location of the keys audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.KeysStream)]
    public string? Keys { get; set; }
    /// <summary>
    /// Location of the drums' kicks audio file
    /// </summary>
    /// <remarks>Can include all drums audio</remarks>
    [ChartKeySerializable(ChartFormatting.DrumStream)]
    public string? Drum { get; set; }
    /// <summary>
    /// Location of the drums' snares audio file
    /// </summary>
    /// <remarks>Can include all drums audio except kicks</remarks>
    [ChartKeySerializable(ChartFormatting.Drum2Stream)]
    public string? Drum2 { get; set; }
    /// <summary>
    /// Location of the drum's toms audio file
    /// </summary>
    /// <remarks>Can include toms and cymbals</remarks>
    [ChartKeySerializable(ChartFormatting.Drum3Stream)]
    public string? Drum3 { get; set; }
    /// <summary>
    /// Location of the drum's cymbals audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.Drum4Stream)]
    public string? Drum4 { get; set; }
    /// <summary>
    /// Location of the vocals audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.VocalStream)]
    public string? Vocals { get; set; }
    /// <summary>
    /// Location of the crowd reaction audio file
    /// </summary>
    [ChartKeySerializable(ChartFormatting.CrowdStream)]
    public string? Crowd { get; set; }
}
