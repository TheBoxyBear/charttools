using ChartTools.Internal;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Ini;

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
    public abstract record Instrument : IEmpty
    {
        /// <inheritdoc cref="IEmpty.IsEmpty"/>
        public bool IsEmpty => GetTracks().All(t => t.IsEmpty);

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
                    InstrumentIdentity.LeadGuitar or InstrumentIdentity.RhythmGuitar or InstrumentIdentity.CoopGuitar or InstrumentIdentity.GHLBass or InstrumentIdentity.Keys => InstrumentType.Standard,
                    InstrumentIdentity.GHLGuitar or InstrumentIdentity.GHLBass => InstrumentType.GHL,
                    InstrumentIdentity.Vocals => InstrumentType.Vocals,
                    _ => throw new InvalidDataException($"Instrument identity {InstrumentIdentity} does not belong to an instrument type.")
                };

                return _instrumentType.Value;
            }
        }
        private InstrumentType? _instrumentType;

        /// <summary>
        /// Estimated difficulty
        /// </summary>
        public sbyte? Difficulty { get; set; }

        /// <summary>
        /// Easy track
        /// </summary>
        public Track Easy
        {
            get => _easy;
            protected set => _easy = value;
        }
        private Track _easy;
        /// <summary>
        /// Medium track
        /// </summary>
        public Track Medium
        {
            get => _medium;
            protected set => _medium = value;
        }
        private Track _medium;
        /// <summary>
        /// Hard track
        /// </summary>
        public Track Hard
        {
            get => _hard;
            protected set => _hard = value;
        }
        private Track _hard;
        /// <summary>
        /// Expert track
        /// </summary>
        public Track Expert
        {
            get => _expert;
            protected set => _expert = value;
        }
        private Track _expert;

        /// <summary>
        /// Gets the track matching a difficulty.
        /// </summary>
        public virtual Track GetTrack(DiffEnum difficulty) => difficulty switch
        {
            DiffEnum.Easy => Easy,
            DiffEnum.Medium => Medium,
            DiffEnum.Hard => Hard,
            DiffEnum.Expert => Expert,
            _ => throw CommonExceptions.GetUndefinedException(difficulty)
        };

        /// <summary>
        /// Creates an array containing all tracks.
        /// </summary>
        public virtual Track[] GetTracks() => new Track[] { Easy, Medium, Hard, Expert };
        /// <summary>
        /// Creates an array containing all tracks with data.
        /// </summary>
        public virtual Track[] GetNonEmptyTracks() => GetTracks().Where(t => !t.IsEmpty).ToArray();

        /// <summary>
        /// Gives all tracks the same local events.
        /// </summary>
        public void ShareLocalEvents(TrackObjectSource source) => ShareEventsStarPower(source, track => track.LocalEvents);
        /// <summary>
        /// Gives all tracks the same star power
        /// </summary>
        public void ShareStarPower(TrackObjectSource source) => ShareEventsStarPower(source, track => track.StarPower);
        private void ShareEventsStarPower<T>(TrackObjectSource source, Func<Track, List<T>> collectionGetter) where T : TrackObject
        {
            if (source == TrackObjectSource.Seperate)
                return;

            var collections = GetTracks().Select(track => collectionGetter(track)).ToArray();

            var objects = (source switch
            {
                TrackObjectSource.Easy => collections[0],
                TrackObjectSource.Medium => collections[1],
                TrackObjectSource.Hard => collections[2],
                TrackObjectSource.Expert => collections[3],
                TrackObjectSource.Merge => collections.SelectMany(col => col).Distinct(),
                _ => throw CommonExceptions.GetUndefinedException(source)
            }).ToArray();

            foreach (var collection in collections)
            {
                collection.Clear();
                collection.AddRange(objects);
            }
        }

        #region File reading
        /// <summary>
        /// Reads an instrument from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        /// <param name="instrument">Instrument to read</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        public static Instrument? FromFile(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartReader.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        /// <summary>
        /// Reads an instrument from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="FromFile(string, InstrumentIdentity, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Instrument?> FromFileAsync(string path, InstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadInstrumentAsync(path, instrument, token, config)));

        /// <summary>
        /// Reads drums from a file.
        /// </summary>
        public static Instrument<DrumsChord>? FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", ChartReader.ReadDrums));
        /// <summary>
        /// Reads drums from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<Instrument<DrumsChord>?> FromFileAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Instrument<DrumsChord>?>(path, cancellationToken, config, (".chart", ChartReader.ReadDrumsAsync));

        /// <summary>
        /// Reads a GHL instrument from a file.
        /// </summary>
        public static Instrument<GHLChord>? FromFile(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartReader.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        /// <summary>
        /// Reads a GHL instrument from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<Instrument<GHLChord>?> FromFileAsync(string path, GHLInstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? await ExtensionHandler.ReadAsync(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadInstrumentAsync(path, instrument, token, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);

        /// <summary>
        /// Reads a standard instrument from a file.
        /// </summary>
        public static Instrument<StandardChord>? FromFile(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartReader.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        /// <summary>
        /// Reads a standard instrument from a file asynchronously using multitasking.
        /// </summary>
        public static async Task<Instrument<StandardChord>?> FromFileAsync(string path, StandardInstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? await ExtensionHandler.ReadAsync(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadInstrumentAsync(path, instrument, token, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        #endregion

        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartWriter.ReplaceInstrument));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync(path, this, cancellationToken, config, (".chart", ChartWriter.ReplaceInstrumentAsync));

        /// <inheritdoc cref="IniParser_old.ReadDifficulty(string, InstrumentIdentity)"/>
        public void ReadDifficulty(string path) => ExtensionHandler.Read(path, (".ini", path => IniParser_old.ReadDifficulty(path, this)));
        /// <inheritdoc cref="IniParser_old.WriteDifficulty(string, InstrumentIdentity, sbyte)"/>
        public void WriteDifficulty(string path) => ExtensionHandler.Write(path, (".ini", path => IniParser_old.WriteDifficulty(path, this)));
    }
}
