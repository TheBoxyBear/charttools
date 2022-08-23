using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Ini;
using ChartTools.Lyrics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using ChartTools.Extensions.Linq;
using System.Threading.Tasks;
using System.Threading;
using ChartTools.IO.Configuration;
using ChartTools.Events;
using ChartTools.Formatting;
using ChartTools.IO.Sections;
using ChartTools.Tools;

namespace ChartTools
{
    /// <summary>
    /// Song playable in Clone Hero
    /// </summary>
    public class Song
    {
        /// <summary>
        /// Set of information about the song not unrelated to instruments, syncing or events
        /// </summary>
        public Metadata Metadata
        {
            get => _metadata;
            set => _metadata = value ?? throw new ArgumentNullException(nameof(value));
        }
        private Metadata _metadata = new();

        /// <inheritdoc cref="FormattingRules"/>
        public FormattingRules Formatting
        {
            get => _formatting;
            set => _formatting = value ?? throw new ArgumentNullException(nameof(value));
        }
        private FormattingRules _formatting = new();

        /// <inheritdoc cref="ChartTools.SyncTrack"/>
        public SyncTrack SyncTrack
        {
            get => _syncTrack;
            set => _syncTrack = value ?? throw new ArgumentNullException(nameof(value));
        }
        private SyncTrack _syncTrack = new();

        /// <summary>
        /// List of events common to all instruments
        /// </summary>
        public List<GlobalEvent> GlobalEvents
        {
            get => _globalEvents;
            set => _globalEvents = value ?? throw new ArgumentNullException(nameof(value));
        }
        private List<GlobalEvent> _globalEvents = new();

        /// <inheritdoc cref="InstrumentSet"/>
        public InstrumentSet Instruments
        {
            get => _instruments;
            set => _instruments = value ?? throw new ArgumentNullException(nameof(value));
        }
        private InstrumentSet _instruments = new();

        public ChartSection? UnknownChartSections { get; set; }

        #region Reading
        /// <summary>
        /// Reads all elements of a <see cref="Song"/> from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        public static Song FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadSong(path, config, formatting)), (".ini", path => new Song { Metadata = IniFile.ReadMetadata(path) }));
        /// <summary>
        /// Reads all elements of a <see cref="Song"/> from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="config"><inheritdoc cref="FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<Song> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Song>(path, (".chart", path => ChartFile.ReadSongAsync(path, config, formatting, cancellationToken)));

        public static Song FromDirectory(string directory, ReadingConfiguration? config = default)
        {
            (var song, var metadata) = DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, config, formatting));
            song ??= new();

            PropertyMerger.Merge(song.Metadata, true, true, metadata);

            return song;
        }
        public static async Task<Song> FromDirectoryAsync(string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default)
        {
            (var song, var metadata) = await DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, config, formatting, cancellationToken), cancellationToken);
            song ??= new();

            PropertyMerger.Merge(song.Metadata, true, true, metadata);

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
        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, (".chart", (path, song) => ChartFile.WriteSong(path, song, config)));
        public async Task ToFileAsync(string path, WritingConfiguration? config = default, CancellationToken cancellationToken = default) => await ExtensionHandler.WriteAsync(path, this, (".chart", (path, song) => ChartFile.WriteSongAsync(path, song, config, cancellationToken)));

        /// <summary>
        /// Retrieves the lyrics from the global events.
        /// </summary>
        public IEnumerable<Phrase> GetLyrics() => GlobalEvents is null ? Enumerable.Empty<Phrase>() : GlobalEvents.GetLyrics();
        /// <summary>
        /// Replaces phrase and lyric events from <see cref="GlobalEvents"/> with the ones making up a set of <see cref="Phrase"/>.
        /// </summary>
        /// <param name="phrases">Phrases to use as a replacement</param>
        public void SetLyrics(IEnumerable<Phrase> phrases) => GlobalEvents = (GlobalEvents ?? new()).SetLyrics(phrases).ToList();
    }
}
