using ChartTools.Animations;
using ChartTools.Events;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;
using ChartTools.Lyrics;
using ChartTools.Tools;

namespace ChartTools;

/// <summary>
/// Song playable in Clone Hero
/// </summary>
public class Song
{
    /// <summary>
    /// Set of information about the song not unrelated to instruments, syncing or events
    /// </summary>
    public Metadata Metadata { get; set; } = new();

    /// <inheritdoc cref="SyncTrack"/>
    public SyncTrack SyncTrack { get; set; } = new();

    /// <summary>
    /// List of events common to all instruments
    /// </summary>
    public List<GlobalEvent> GlobalEvents { get; set; } = [];

    /// <inheritdoc cref="InstrumentSet"/>
    public InstrumentSet Instruments { get; set; } = new();

    public AnimationSet Animations { get; set; } = new();

    public ChartSection? UnknownChartSections { get; set; }

    #region Reading
    /// <summary>
    /// Reads all elements of a <see cref="Song"/> from a file.
    /// </summary>
    /// <param name="path">Path of the file</param>
    /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
    /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
    public static Song FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadSong(path, config?.Chart, formatting)), (".ini", path => new Song { Metadata = IniFile.ReadMetadata(path) }));
    /// <summary>
    /// Reads all elements of a <see cref="Song"/> from a file asynchronously using multitasking.
    /// </summary>
    /// <param name="path"><inheritdoc cref="FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
    /// <param name="config"><inheritdoc cref="FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
    /// <param name="cancellationToken">Token to request cancellation</param>
    public static async Task<Song> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Song>(path, (".chart", path => ChartFile.ReadSongAsync(path, config?.Chart, formatting, cancellationToken)));

    public static Song FromDirectory(string directory, ReadingConfiguration? config = default)
    {
        var iniPath = directory + @"\song.ini";
        var chartPath = directory + @"\notes.chart";
        var iniMetadata = File.Exists(iniPath) ? IniFile.ReadMetadata(iniPath) : new();

        Song? song = null;

        if (File.Exists(chartPath))
            song = FromFile(chartPath, config, iniMetadata.Formatting);

        song ??= new();

        PropertyMerger.Merge(song.Metadata, true, true, iniMetadata);

        return song;
    }
    public static async Task<Song> FromDirectoryAsync(string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default)
    {
        var iniPath = directory + @"\song.ini";
        var chartPath = directory + @"\notes.chart";
        var iniMetadata = File.Exists(iniPath) ? await IniFile.ReadMetadataAsync(iniPath, null, cancellationToken) : new();

        Song? song = null;

        if (File.Exists(chartPath))
            song = await FromFileAsync(chartPath, config, iniMetadata.Formatting, cancellationToken);

        song ??= new();

        PropertyMerger.Merge(song.Metadata, true, true, iniMetadata);

        return song;
    }
    #endregion

    /// <summary>
    /// Writes the <see cref="Song"/> to a file.
    /// </summary>
    /// <exception cref="ArgumentException"/>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="PathTooLongException"/>
    /// <exception cref="DirectoryNotFoundException"/>
    /// <exception cref="LineException"/>
    /// <exception cref="UnauthorizedAccessException"/>
    /// <exception cref="NotSupportedException"/>
    /// <exception cref="System.Security.SecurityException"/>
    public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, (".chart", (path, song) => ChartFile.WriteSong(path, song, config?.Chart)));
    public async Task ToFileAsync(string path, WritingConfiguration? config = default, CancellationToken cancellationToken = default) => await ExtensionHandler.WriteAsync(path, this, (".chart", (path, song) => ChartFile.WriteSongAsync(path, song, config?.Chart, cancellationToken)));

    /// <summary>
    /// Retrieves the lyrics from the global events.
    /// </summary>
    public IEnumerable<Phrase> GetLyrics() => GlobalEvents is null ? Enumerable.Empty<Phrase>() : GlobalEvents.GetLyrics();
    /// <summary>
    /// Replaces phrase and lyric events from <see cref="GlobalEvents"/> with the ones making up a set of <see cref="Phrase"/>.
    /// </summary>
    /// <param name="phrases">Phrases to use as a replacement</param>
    public void SetLyrics(IEnumerable<Phrase> phrases) => GlobalEvents = (GlobalEvents ?? []).SetLyrics(phrases).ToList();
}