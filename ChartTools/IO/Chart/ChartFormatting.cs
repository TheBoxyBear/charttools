using ChartTools.Extensions;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Chart;

internal static class ChartFormatting
{
    public const string DrumsHeaderName = "Drums",
        MetadataHeader = "[Song]",
        SyncTrackHeader = "[SyncTrack]",
        GlobalEventHeader = "[Events]",
        Title = "Name",
        Artist = "Artist",
        Charter = "Charter",
        Album = "Album",
        Year = "Year",
        AudioOffset = "Offset",
        Resolution = "Resolution",
        Difficulty = "Difficulty",
        PreviewStart = "PreviewStart",
        PreviewEnd = "PreviewEnd",
        Genre = "Genre",
        MediaType = "MediaType",
        MusicStream = "MusicStream",
        GuitarStream = "GuitarStream",
        BassStream = "BassStream",
        RhythmStream = "RhythmStream",
        KeysStream = "KeysStream",
        DrumStream = "DrumStream",
        Drum2Stream = "Drum2Stream",
        Drum3Stream = "Drum3Stream",
        Drum4Stream = "Drum4Stream",
        VocalStream = "VocalStream",
        CrowdStream = "CrowdStream";

    /// <summary>
    /// Part names of <see cref="InstrumentIdentity"/> without the difficulty
    /// </summary>
    public static readonly IReadOnlyDictionary<InstrumentIdentity, string> InstrumentHeaderNames = new Dictionary<InstrumentIdentity, string>()
    {
        { InstrumentIdentity.Drums, DrumsHeaderName },
        { InstrumentIdentity.GHLGuitar, "GHLGuitar" },
        { InstrumentIdentity.GHLBass, "GHLBass" },
        { InstrumentIdentity.LeadGuitar, "Single" },
        { InstrumentIdentity.RhythmGuitar, "DoubleRhythm" },
        { InstrumentIdentity.CoopGuitar, "DoubleGuitar" },
        { InstrumentIdentity.Bass, "DoubleBass" },
        { InstrumentIdentity.Keys, "Keyboard" }
    };

    /// <summary>
    /// Headers for drums tracks
    /// </summary>
    public static readonly IReadOnlyDictionary<string, Difficulty> DrumsTrackHeaders =
        EnumCache<Difficulty>.Values
        .ToDictionary(diff => Header(DrumsHeaderName, diff));

    /// <summary>
    /// Headers for GHL tracks
    /// </summary>
    public static readonly IReadOnlyDictionary<string, (Difficulty, GHLInstrumentIdentity)> GHLTrackHeaders =
        GetTrackCombinations(Enum.GetValues<GHLInstrumentIdentity>())
        .ToDictionary(tuple => Header(tuple.instrument, tuple.difficulty));

    /// <summary>
    /// Headers for standard tracks
    /// </summary>
    public static readonly Dictionary<string, (Difficulty, StandardInstrumentIdentity)> StandardTrackHeaders =
        GetTrackCombinations(Enum.GetValues<StandardInstrumentIdentity>())
        .ToDictionary(tuple => Header((InstrumentIdentity)tuple.instrument, tuple.difficulty));

    /// <summary>
    /// Gets all the combinations of instruments and difficulties.
    /// </summary>
    /// <param name="instruments">Enum containing the instruments</param>
    private static IEnumerable<(Difficulty difficulty, TInstEnum instrument)> GetTrackCombinations<TInstEnum>(IEnumerable<TInstEnum> instruments)
        => from difficulty in EnumCache<Difficulty>.Values from instrument in instruments select (difficulty, instrument);

    public static string Header(Enum instrument, Difficulty difficulty) => Header((InstrumentIdentity)instrument, difficulty);
    public static string Header(InstrumentIdentity instrument, Difficulty difficulty) => Header(InstrumentHeaderNames[instrument], difficulty);
    public static string Header(string instrumentName, Difficulty difficulty) => Header(difficulty.ToString() + instrumentName);
    public static string Header(string name) => $"[{name}]";

    public static string Line(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";

    /// <summary>
    /// Gets the written data for a note.
    /// </summary>
     /// <param name="position">Position of the parent <see cref="LaneChord"/></param>
    /// <param name="index">Value of <see cref="LaneNote.Index"/></param>
    /// <param name="sustain">Value of <see cref="LaneNote.Sustain"/></param>
    public static TrackObjectEntry NoteEntry(uint position, byte index, uint sustain) => new(position, "N", $"{index} {sustain}");

    /// <summary>
    /// Gets the written value of a float.
    /// </summary>
    /// <param name="value">Value to get the written equivalent of</param>
    public static string Float(float value) => ((int)(value * 1000)).ToString().Replace(".", "").Replace(",", "");

    public static bool IsSectionEnd(string line) => line == "}";

    /// <summary>
    /// Splits the data of an entry.
    /// </summary>
    /// <param name="data">Data portion of a <see cref="Entries.TrackObjectEntry"/></param>
    internal static string[] SplitData(string data) => data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
}
