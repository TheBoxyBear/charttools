using ChartTools.Internal;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Ini;

using System;
using System.Collections.Generic;
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
        public bool IsEmpty => GetTracks().All(t => t.IsEmpty);

        public InstrumentIdentity InstrumentIdentity { get; init; }
        public InstrumentType InstrumentType { get; init; }

        /// <summary>
        /// Estimated difficulty
        /// </summary>
        public sbyte? Difficulty { get; set; }

        /// <summary>
        /// Easy track
        /// </summary>
        public abstract Track Easy { get; }
        /// <summary>
        /// Medium track
        /// </summary>
        public abstract Track Medium { get; }
        /// <summary>
        /// Hard track
        /// </summary>
        public abstract Track Hard { get; }
        /// <summary>
        /// Expert track
        /// </summary>
        public abstract Track Expert { get; }
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
        public virtual Track[] GetTracks() => new Track[] { Easy, Medium, Hard, Expert };
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
        /// <inheritdoc cref="ChartReader.ReadInstrument(string, InstrumentIdentity, ReadingConfiguration)"/>
        public static Instrument FromFile(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartReader.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);

        /// <summary>
        /// Reads drums from a file.
        /// </summary>
        /// <inheritdoc cref="ChartReader.ReadDrums(string, ReadingConfiguration)"/>
        public static Instrument<DrumsChord>? FromFile(string path, ReadingConfiguration? config = default) => ExtensionHandler.Read(path, config, (".chart", ChartReader.ReadDrums));
        public static async Task<Instrument<DrumsChord>?> FromFileAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config = default) => await ExtensionHandler.ReadAsync<Instrument<DrumsChord>?>(path, cancellationToken, config, (".chart", ChartReader.ReadDrumsAsync));

        /// <summary>
        /// Reads a GHL instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartReader.ReadInstrument(string, GHLInstrumentIdentity, ReadingConfiguration)" />
        public static Instrument<GHLChord>? FromFile(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartReader.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        public static async Task<Instrument<GHLChord>>? FromFileAsync(string path, GHLInstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? await ExtensionHandler.ReadAsync(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadInstrumentAsync(path, instrument, token, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);

        /// <summary>
        /// Reads a standard instrument from a file.
        /// </summary>
        /// <inheritdoc cref="ChartReader.ReadInstrument(string, StandardInstrumentIdentity, ReadingConfiguration)"/>
        public static Instrument<StandardChord> FromFile(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? ExtensionHandler.Read(path, config, (".chart", (p, config) => ChartReader.ReadInstrument(p, instrument, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        public static async Task<Instrument<StandardChord>>? FromFileAsync(string path, StandardInstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default) => Enum.IsDefined(instrument)
            ? await ExtensionHandler.ReadAsync(path, cancellationToken, config, (".chart", (path, token, config) => ChartReader.ReadInstrumentAsync(path, instrument, token, config)))
            : throw CommonExceptions.GetUndefinedException(instrument);
        #endregion

        public void ToFile(string path, WritingConfiguration? config = default) => ExtensionHandler.Write(path, this, config, (".chart", ChartWriter.ReplaceInstrument));
        public async Task ToFileAsync(string path, CancellationToken cancellationToken, WritingConfiguration? config = default) => await ExtensionHandler.WriteAsync(path, this, cancellationToken, config, (".chart", ChartWriter.ReplaceInstrumentAsync));

        /// <inheritdoc cref="IniParser.ReadDifficulty(string, InstrumentIdentity)"/>
        public void ReadDifficulty(string path) => ExtensionHandler.Read(path, (".ini", path => IniParser.ReadDifficulty(path, this)));
        /// <inheritdoc cref="IniParser.WriteDifficulty(string, InstrumentIdentity, sbyte)"/>
        public void WriteDifficulty(string path) => ExtensionHandler.Write(path, (".ini", path => IniParser.WriteDifficulty(path, this)));
    }
}
