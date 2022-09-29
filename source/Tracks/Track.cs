using ChartTools.Events;
using ChartTools.IO.Formatting;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools
{
    /// <summary>
    /// Base class for tracks
    /// </summary>
    public abstract record Track : IEmptyVerifiable
    {
        /// <inheritdoc cref="IEmptyVerifiable.IsEmpty"/>
        public bool IsEmpty => Chords.Count == 0 && LocalEvents.Count == 0 && SpecialPhrases.Count == 0;

        /// <summary>
        /// Difficulty of the track
        /// </summary>
        public Difficulty Difficulty { get; init; }
        /// <summary>
        /// Instrument containing the track
        /// </summary>
        public Instrument? ParentInstrument => GetInstrument();
        /// <summary>
        /// Events specific to the <see cref="Track"/>
        /// </summary>
        public List<LocalEvent> LocalEvents { get; } = new();
        /// <summary>
        /// Set of special phrases
        /// </summary>
        public List<TrackSpecialPhrase> SpecialPhrases { get; } = new();

        /// <summary>
        /// Groups of notes of the same position
        /// </summary>
        public IReadOnlyList<IChord> Chords => GetChords();

        protected abstract IReadOnlyList<IChord> GetChords();

        public abstract IChord CreateChord(uint position);

        internal IEnumerable<TrackSpecialPhrase> SoloToStarPower(bool removeEvents)
        {
            if (LocalEvents is null)
                yield break;

            foreach (LocalEvent e in LocalEvents.OrderBy(e => e.Position))
            {
                TrackSpecialPhrase? phrase = null;

                switch (e.EventType)
                {
                    case EventTypeHelper.Local.Solo:
                        phrase = new(e.Position, TrackSpecialPhraseType.StarPowerGain);
                        break;
                    case EventTypeHelper.Local.SoloEnd:
                        if (phrase is not null)
                        {
                            phrase.Length = e.Position - phrase.Position;
                            yield return phrase;
                            phrase = null;
                        }
                        break;
                }
            }

            if (removeEvents)
                LocalEvents.RemoveAll(e => e.IsSoloEvent);
        }

        protected abstract Instrument? GetInstrument();

        #region File reading
        #region Single file
        /// <summary>
        /// Reads a track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        public static Track FromFile(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read<Track>(path, (".chart", path => ChartFile.ReadTrack(path, instrument, difficulty, config, formatting)));
        /// <summary>
        /// Reads a track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        /// <param name="formatting"><inheritdoc cref="FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='formatting']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<Track> FromFileAsync(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Track>(path, (".chart", path => ChartFile.ReadTrackAsync(path, instrument, difficulty, config, formatting, cancellationToken)));

        /// <summary>
        /// Reads a drums track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        public static Track<DrumsChord> FromFile(string path, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read<Track<DrumsChord>>(path, (".chart", path => ChartFile.ReadDrumsTrack(path, difficulty, config, formatting)));
        /// <summary>
        /// Reads a drums track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<Track<DrumsChord>> FromFileAsync(string path, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Track<DrumsChord>>(path, (".chart", path => ChartFile.ReadDrumsTrackAsync(path, difficulty, config, formatting, cancellationToken)));

        /// <summary>
        /// Reads a GHL track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">GHL instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        public static Track<GHLChord> FromFile(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read<Track<GHLChord>>(path, (".chart", path => ChartFile.ReadTrack(path, instrument, difficulty, config, formatting)));
        /// <summary>
        /// Reads a GHL track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        /// <param name="formatting"><inheritdoc cref="FromFile(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='formatting']"/></param>
        /// <returns></returns>
        public static async Task<Track<GHLChord>> FromFileAsync(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Track<GHLChord>>(path, (".chart", path => ChartFile.ReadTrackAsync(path, instrument, difficulty, config, formatting, cancellationToken)));

        /// <summary>
        /// Reads a standard track from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">Standard instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Track<StandardChord> FromFile(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read<Track<StandardChord>>(path, (".chart", path => ChartFile.ReadTrack(path, instrument, difficulty, config, formatting)));
        /// <summary>
        /// Reads a track from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration, FormattingRules)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration, FormattingRules)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration, FormattingRules)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        /// <param name="config"><inheritdoc cref="FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static async Task<Track<StandardChord>> FromFileAsync(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Track<StandardChord>>(path, (".chart", path => ChartFile.ReadTrackAsync(path, instrument, difficulty, config, formatting, cancellationToken)));
        #endregion

        #region Directory
        public static DirectoryResult<Track?> FromDirectory(string directory, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, difficulty, config, formatting));
        public static async Task<DirectoryResult<Track?>> FromDirectoryAsync(string directory, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => await DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, difficulty, config, formatting, cancellationToken), cancellationToken);

        public static DirectoryResult<Track<DrumsChord>?> FromDirectory(string directory, Difficulty difficulty, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, difficulty, config, formatting));
        public static async Task<DirectoryResult<Track<DrumsChord>?>> FromDirectoryAsync(string directory, Difficulty difficulty, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => await DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, difficulty, config, formatting, cancellationToken), cancellationToken);

        public static DirectoryResult<Track<GHLChord>?> FromDirectory(string directory, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, difficulty, config, formatting));
        public static async Task<DirectoryResult<Track<GHLChord>?>> FromDirectoryAsync(string directory, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => await DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, difficulty, config, formatting, cancellationToken), cancellationToken);

        public static DirectoryResult<Track<StandardChord>?> FromDirectory(string directory, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, difficulty, config, formatting));
        public static async Task<DirectoryResult<Track<StandardChord>?>> FromDirectoryAsync(string directory, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => await DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, difficulty, config, formatting, cancellationToken), cancellationToken);
        #endregion
        #endregion

        public void ToFile(string path, WritingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Write<Track>(path, this, (".chart", (path, track) => ChartFile.ReplaceTrack(path, track, config, formatting)));
        public async Task ToFileAsync(string path, WritingConfiguration? config = default,FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.WriteAsync<Track>(path, this, (".chart", (path, track) => ChartFile.ReplaceTrackAsync(path, track, config, formatting, cancellationToken)));

        public override string ToString() => Difficulty.ToString();
    }
}
