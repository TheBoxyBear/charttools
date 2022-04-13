using ChartTools.Exceptions;
using ChartTools.Formatting;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DiffEnum = ChartTools.Difficulty;

namespace ChartTools
{
    /// <summary>
    /// Base class for instruments
    /// </summary>
    public abstract record Instrument
    {
        /// <summary>
        /// Identity of the instrument the object belongs to
        /// </summary>
        public InstrumentIdentity InstrumentIdentity { get; init; }

        /// <summary>
        /// Type of instrument
        /// </summary>
        public InstrumentType InstrumentType
        {
            get
            {
                if (_instrumentType is not null)
                    return _instrumentType.Value;

                _instrumentType = InstrumentIdentity switch
                {
                    InstrumentIdentity.Drums => InstrumentType.Drums,
                    InstrumentIdentity.LeadGuitar or InstrumentIdentity.RhythmGuitar or InstrumentIdentity.Bass or InstrumentIdentity.CoopGuitar or InstrumentIdentity.GHLBass or InstrumentIdentity.Keys => InstrumentType.Standard,
                    InstrumentIdentity.GHLGuitar or InstrumentIdentity.GHLBass => InstrumentType.GHL,
                    InstrumentIdentity.Vocals => InstrumentType.Vocals,
                    _ => throw new InvalidDataException($"Instrument identity {InstrumentIdentity} does not belong to an instrument type.")
                };

                return _instrumentType.Value;
            }
        }
        private InstrumentType? _instrumentType;

        /// <inheritdoc cref="InstrumentDifficultySet.GetDifficulty(InstrumentIdentity)"/>
        public sbyte? GetDifficulty(InstrumentDifficultySet difficulties) => difficulties.GetDifficulty(InstrumentIdentity);
        /// <inheritdoc cref="InstrumentDifficultySet.GetDifficulty(InstrumentIdentity)"/>
        public void SetDifficulty(InstrumentDifficultySet difficulties, sbyte? difficulty) => difficulties.SetDifficulty(InstrumentIdentity, difficulty);

        /// <summary>
        /// Easy track
        /// </summary>
        public Track? Easy => GetEasy();
        /// <summary>
        /// Medium track
        /// </summary>
        public Track? Medium => GetMedium();
        /// <summary>
        /// Hard track
        /// </summary>
        public Track? Hard => GetHard();
        /// <summary>
        /// Expert track
        /// </summary>
        public Track? Expert => GetExpert();

        /// <summary>
        /// Gets the track matching a difficulty.
        /// </summary>
        public abstract Track? GetTrack(DiffEnum difficulty);

        protected abstract Track? GetEasy();
        protected abstract Track? GetMedium();
        protected abstract Track? GetHard();
        protected abstract Track? GetExpert();

        /// <summary>
        /// Creates a track
        /// </summary>
        /// <param name="difficulty">Difficulty of the track</param>
        public abstract Track CreateTrack(DiffEnum difficulty);
        /// <summary>
        /// Removes a track.
        /// </summary>
        /// <param name="difficulty">Difficulty of the target track</param>
        public abstract bool RemoveTrack(DiffEnum difficulty);

        /// <summary>
        /// Creates an array containing all tracks.
        /// </summary>
        public virtual Track?[] GetTracks() => new Track?[] { Easy, Medium, Hard, Expert };
        /// <summary>
        /// Creates an array containing all tracks with data.
        /// </summary>
        public virtual IEnumerable<Track> GetExistingTracks() => GetTracks().NonNull();

        /// <summary>
        /// Gives all tracks the same local events.
        /// </summary>
        public void ShareLocalEvents(TrackObjectSource source) => ShareEventsStarPower(source, track => track.LocalEvents);
        /// <summary>
        /// Gives all tracks the same star power
        /// </summary>
        public void ShareStarPower(TrackObjectSource source) => ShareEventsStarPower(source, track => track.SpecialPhrases);
        private void ShareEventsStarPower<T>(TrackObjectSource source, Func<Track, List<T>> collectionGetter) where T : TrackObject
        {
            if (source == TrackObjectSource.Seperate)
                return;

            var collections = GetExistingTracks().Select(track => collectionGetter(track)).ToArray();

            var objects = (source switch
            {
                TrackObjectSource.Easy => collections[0],
                TrackObjectSource.Medium => collections[1],
                TrackObjectSource.Hard => collections[2],
                TrackObjectSource.Expert => collections[3],
                TrackObjectSource.Merge => collections.SelectMany(col => col).Distinct(),
                _ => throw new UndefinedEnumException(source)
            }).ToArray();

            foreach (var collection in collections)
            {
                collection.Clear();
                collection.AddRange(objects);
            }
        }

        #region File reading
        #region Single file
        /// <summary>
        /// Reads an instrument from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">Instrument to read</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Instrument? FromFile(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadInstrument(path, instrument, config, formatting)));
        /// <summary>
        /// Reads an instrument from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='instrument']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static async Task<Instrument?> FromFileAsync(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync(path, (".chart", path => ChartFile.ReadInstrumentAsync(path, instrument, config, formatting, cancellationToken)));

        /// <summary>
        /// Reads drums from a file.
        /// </summary>
        public static Instrument<DrumsChord>? FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadDrums(path, config, formatting)));
        /// <summary>
        /// Reads drums from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<Instrument<DrumsChord>?> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Instrument<DrumsChord>?>(path, (".chart", path => ChartFile.ReadDrumsAsync(path, config, formatting, cancellationToken)));

        /// <summary>
        /// Reads a GHL instrument from a file.
        /// </summary>
        public static Instrument<GHLChord>? FromFile(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadInstrument(path, instrument, config, formatting)));
        /// <summary>
        /// Reads a GHL instrument from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<Instrument<GHLChord>?> FromFileAsync(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync(path, (".chart", path => ChartFile.ReadInstrumentAsync(path, instrument, config, formatting, cancellationToken)));

        /// <summary>
        /// Reads a standard instrument from a file.
        /// </summary>
        public static Instrument<StandardChord>? FromFile(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(instrument);
            return ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadInstrument(path, instrument, config, formatting)));
        }
        /// <summary>
        /// Reads a standard instrument from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<Instrument<StandardChord>?> FromFileAsync(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync(path, (".chart", path => ChartFile.ReadInstrumentAsync(path, instrument, config, formatting, cancellationToken)));
        #endregion

        #region Directory
        public static DirectoryResult<Instrument?> FromDirectory(string directory, InstrumentIdentity instrument, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, config, formatting));
        public static Task<DirectoryResult<Instrument?>> FromDirectoryAsync(string directory, InstrumentIdentity instrument, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, config, formatting, cancellationToken), cancellationToken);

        public static DirectoryResult<Instrument<DrumsChord>?> FromDirectory(string directory, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, config, formatting));
        public static Task<DirectoryResult<Instrument<DrumsChord>?>> FromDirectoryAsync(string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, config, formatting, cancellationToken), cancellationToken);

        public static DirectoryResult<Instrument<GHLChord>?> FromDirectory(string directory, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, config, formatting));
        public static Task<DirectoryResult<Instrument<GHLChord>?>> FromDirectoryAsync(string directory, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, config, formatting, cancellationToken), cancellationToken);

        public static DirectoryResult<Instrument<StandardChord>?> FromDirectory(string directory, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, config, formatting));
        public static Task<DirectoryResult<Instrument<StandardChord>?>> FromDirectoryAsync(string directory, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, config, formatting, cancellationToken), cancellationToken);
        #endregion
        #endregion

        public void ToFile(string path, WritingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Write(path, this, (".chart", (path, inst) => ChartFile.ReplaceInstrument(path, inst, config, formatting)));
        public async Task ToFileAsync(string path, WritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.WriteAsync(path, this, (".chart", (path, inst) => ChartFile.ReplaceInstrumentAsync(path, inst, config, formatting, cancellationToken)));
    }
}
