using ChartTools.IO.Chart.Parsers;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.Lyrics;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Provides methods for reading chart files
    /// </summary>
    public static partial class ChartReader
    {
        /// <summary>
        /// Default configuration to use when the provided configuration is <see langword="default"/>
        /// </summary>
        public static ReadingConfiguration DefaultConfig = new()
        {
            DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
        };

        #region Song
        /// <summary>
        /// Creates a <see cref="ChartParser"/> for parsing a section based on the header.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static ChartParser? GetSongParser(string header, ReadingSession session)
        {
            switch (header)
            {
                case ChartFormatting.MetadataHeader:
                    return new MetadataParser(session);
                case ChartFormatting.GlobalEventHeader:
                    return new GlobalEventParser(session);
                case ChartFormatting.SyncTrackHeader:
                    return new SyncTrackParser(session);
                default:
                    if (drumsTrackHeaders.TryGetValue(header, out Difficulty diff))
                        return new DrumsTrackParser(diff, session);
                    else if (ghlTrackHeaders.TryGetValue(header, out (Difficulty, GHLInstrumentIdentity) ghlTuple))
                        return new GHLTrackParser(ghlTuple.Item1, ghlTuple.Item2, session);
                    else if (standardTrackHeaders.TryGetValue(header, out (Difficulty, StandardInstrumentIdentity) standardTuple))
                        return new StandardTrackParser(standardTuple.Item1, standardTuple.Item2, session);
                    else
                        throw new FormatException($"Unknown part: {header}"); // TODO Add support for unknown parts in configuration
            }
        }

        /// <summary>
        /// Combines the results from the parsers of a <see cref="ChartFileReader"/> into a <see cref="Song"/>.
        /// </summary>
        /// <param name="reader">Reader to get the parsers from</param>
        private static Song CreateSongFromReader(ChartFileReader reader)
        {
            Song song = new();

            foreach (var parser in reader.Parsers)
                parser.ApplyResultToSong(song);

            return song;
        }

        /// <inheritdoc cref="Song.FromFile(string, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="config"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        /// <returns></returns>
        public static Song ReadSong(string path, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetSongParser(header, new(config ?? DefaultConfig)));
            reader.Read();
            return CreateSongFromReader(reader);
        }

        /// <inheritdoc cref="Song.FromFileAsync(string, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Song.FromFileAsync(string, CancellationToken, ReadingConfiguration?)" path="/param[@='path']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Song.FromFileAsync(string, CancellationToken, ReadingConfiguration?)" path="/param[@='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Song.FromFileAsync(string, CancellationToken, ReadingConfiguration?)" path="/param[@='config']"/></param>
        public static async Task<Song> ReadSongAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config)
        {
            var reader = new ChartFileReader(path, header => GetSongParser(header, new(config ?? DefaultConfig)));
            await reader.ReadAsync(cancellationToken);
            return CreateSongFromReader(reader);
        }
        #endregion
        #region Instruments
        /// <summary>
        /// Combines the results from the parsers in a <see cref="ChartFileReader"/> into an instrument.
        /// </summary>
        private static Instrument<TChord>? CreateInstrumentFromReader<TChord>(ChartFileReader reader) where TChord : Chord
        {
            Instrument<TChord>? output = null;

            foreach (TrackParser<TChord> parser in reader.Parsers)
                (output ??= new()).SetTrack(parser.Result!, parser.Difficulty);

            return output;
        }

        /// <summary>
        /// Reads an instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument to read</param>
        /// <inheritdoc cref="ReadDrums(string, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, GHLInstrumentIdentity, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, StandardInstrumentIdentity, ReadingConfiguration)" path="/exception"/>
        public static Instrument? ReadInstrument(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return ReadDrums(path, config);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return ReadInstrument(path, (GHLInstrumentIdentity)instrument, config);
            return Enum.IsDefined((StandardInstrumentIdentity)instrument)
                ? ReadInstrument(path, (StandardInstrumentIdentity)instrument, config)
                : throw CommonExceptions.GetUndefinedException(instrument);
        }
        public static async Task<Instrument?> ReadInstrumentAsync(string path, InstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return await ReadDrumsAsync(path, cancellationToken, config);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return await ReadInstrumentAsync(path, (GHLInstrumentIdentity)instrument, cancellationToken, config);
            return Enum.IsDefined((StandardInstrumentIdentity)instrument)
                ? await ReadInstrumentAsync(path, (StandardInstrumentIdentity)instrument, cancellationToken, config)
                : throw CommonExceptions.GetUndefinedException(instrument);
        }
        #region Drums
        private static DrumsTrackParser? GetAnyDrumsTrackParser(string header, ReadingSession session) => drumsTrackHeaders.TryGetValue(header, out Difficulty difficulty)
            ? new(difficulty, session)
            : null;
        /// <summary>
        /// Reads drums from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all drums data
        ///     <para><see langword="null"/> if the file contains no drums data</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        /// <inheritdoc cref="GetDrumsTrack(IEnumerable{string}, ReadingConfiguration)(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        public static Instrument<DrumsChord>? ReadDrums(string path, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetAnyDrumsTrackParser(header, new(config ?? DefaultConfig)));
            reader.Read();
            return CreateInstrumentFromReader<DrumsChord>(reader);
        }
        public static async Task<Instrument<DrumsChord>?> ReadDrumsAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetAnyDrumsTrackParser(header, new(config ?? DefaultConfig)));
            await reader.ReadAsync(cancellationToken);
            return CreateInstrumentFromReader<DrumsChord>(reader);
        }
        #endregion
        #region GHL
        private static GHLTrackParser? GetAnyGHLTrackParser(string part, GHLInstrumentIdentity instrument, ReadingSession session) => ghlTrackHeaders.TryGetValue(part, out (Difficulty, GHLInstrumentIdentity) tuple) && tuple.Item2 == instrument
            ? new(tuple.Item1, tuple.Item2, session)
            : null;
        /// <summary>
        /// Reads a Guitar Hero Live instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file has no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        /// <inheritdoc cref="GetGHLTrack(IEnumerable{string}, ReadingConfiguration)(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        public static Instrument<GHLChord>? ReadInstrument(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetAnyGHLTrackParser(header, instrument, new(config ?? DefaultConfig)));
            reader.Read();
            return CreateInstrumentFromReader<GHLChord>(reader);
        }
        public static async Task<Instrument<GHLChord>?> ReadInstrumentAsync(string path, GHLInstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetAnyGHLTrackParser(header, instrument, new(config ?? DefaultConfig)));
            await reader.ReadAsync(cancellationToken);
            return CreateInstrumentFromReader<GHLChord>(reader);
        }
        #endregion
        #region Standard
        private static StandardTrackParser? GetAnyStandardTrackParser(string part, StandardInstrumentIdentity instrument, ReadingSession session) => standardTrackHeaders.TryGetValue(part, out (Difficulty, StandardInstrumentIdentity) tuple) && tuple.Item2 == instrument
            ? new(tuple.Item1, tuple.Item2, session)
            : null;
        /// <inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="config"><inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static Instrument<StandardChord>? ReadInstrument(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetAnyStandardTrackParser(header, instrument, new(config ?? DefaultConfig)));
            reader.Read();
            return CreateInstrumentFromReader<StandardChord>(reader);
        }
        /// <inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, CancellationToken, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, CancellationToken, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, CancellationToken, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, CancellationToken, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Instrument<StandardChord>?> ReadInstrumentAsync(string path, StandardInstrumentIdentity instrument, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            var reader = new ChartFileReader(path, header => GetAnyStandardTrackParser(header, instrument, new(config ?? DefaultConfig)));
            await reader.ReadAsync(cancellationToken);
            return CreateInstrumentFromReader<StandardChord>(reader);
        }
        #endregion
        #endregion
        #region Tracks
        /// <inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static Track ReadTrack(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return ReadDrumsTrack(path, difficulty, config);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return ReadTrack(path, (GHLInstrumentIdentity)instrument, difficulty, config);
            if (Enum.IsDefined((StandardInstrumentIdentity)instrument))
                return ReadTrack(path, (StandardInstrumentIdentity)instrument, difficulty, config);

            throw CommonExceptions.GetUndefinedException(instrument);
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Track> ReadTrackAsync(string path, InstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return await ReadDrumsTrackAsync(path, difficulty, cancellationToken, config);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return await ReadTrackAsync(path, (GHLInstrumentIdentity)instrument, difficulty, cancellationToken, config);
            if (Enum.IsDefined((StandardInstrumentIdentity)instrument))
                return await ReadTrackAsync(path, (StandardInstrumentIdentity)instrument, difficulty, cancellationToken, config);

            throw CommonExceptions.GetUndefinedException(instrument);
        }
        #region Drums
        /// <summary>
        /// Creates a <see cref="DrumsTrackParser"/> is the header matches the requested standard track, otherwise <see langword="null"/>.
        /// </summary>
        /// <param name="header">Header of the part</param>
        /// <param name="seekedHeader">Header to compare against</param>
        /// <param name="difficulty">Difficulty identity to provide the parser</param>
        /// <param name="session">Session to provide the parser</param>
        private static DrumsTrackParser? GetDrumsTrackParser(string header, string seekedHeader, Difficulty difficulty, ReadingSession session) => header == seekedHeader ? new(difficulty, session) : null;
        /// <summary>
        /// Headers for drums tracks
        /// </summary>
        private static readonly Dictionary<string, Difficulty> drumsTrackHeaders = Enum.GetValues<Difficulty>().ToDictionary(diff => ChartFormatting.Header(ChartFormatting.DrumsHeaderName, diff));
        /// <inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static Track<DrumsChord> ReadDrumsTrack(string path, Difficulty difficulty, ReadingConfiguration? config = default)
        {
            var seekedHeader = ChartFormatting.Header(InstrumentIdentity.Drums, difficulty);
            var reader = new ChartFileReader(path, header => GetDrumsTrackParser(header, seekedHeader, difficulty, new(config ?? DefaultConfig)));

            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out DrumsTrackParser? parser) ? parser!.Result! : new();
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, Difficulty, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Track<DrumsChord>> ReadDrumsTrackAsync(string path, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            var seekedHeader = ChartFormatting.Header(ChartFormatting.DrumsHeaderName, difficulty);
            var reader = new ChartFileReader(path, header => GetDrumsTrackParser(header, seekedHeader, difficulty, new(config ?? DefaultConfig)));

            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out DrumsTrackParser? parser) ? parser!.Result! : new();
        }
        #endregion
        #region GHL
        /// <summary>
        /// Creates a <see cref="GHLTrackParser"/> is the header matches the requested standard track, otherwise <see langword="null"/>.
        /// </summary>
        /// <param name="header">Header of the part</param>
        /// <param name="seekedHeader">Header to compare against</param>
        /// <param name="instrument">Instrument identity to provide the parser</param>
        /// <param name="difficulty">Difficulty identity to provide the parser</param>
        /// <param name="session">Session to provide the parser</param>
        private static GHLTrackParser? GetGHLTrackParser(string header, string seekedHeader, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingSession session) => header == seekedHeader ? new(difficulty, instrument, session) : null;
        /// <summary>
        /// Headers for GHL tracks
        /// </summary>
        private static readonly Dictionary<string, (Difficulty, GHLInstrumentIdentity)> ghlTrackHeaders = GetTrackCombinations(Enum.GetValues<GHLInstrumentIdentity>()).ToDictionary(tuple => ChartFormatting.Header(tuple.instrument, tuple.difficulty));
        /// <inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static Track<GHLChord> ReadTrack(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default)
        {
            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var reader = new ChartFileReader(path, header => GetGHLTrackParser(header, seekedHeader, instrument, difficulty, new(config ?? DefaultConfig)));
            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out GHLTrackParser? parser) ? parser!.Result! : new();
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static async Task<Track<GHLChord>> ReadTrackAsync(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config)
        {
            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var reader = new ChartFileReader(path, header => GetGHLTrackParser(header, seekedHeader, instrument, difficulty, new(config ?? DefaultConfig)));

            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out GHLTrackParser? parser) ? parser!.Result! : new();
        }
        #endregion
        #region Standard
        /// <summary>
        /// Creates a <see cref="StandardTrackParser"/> is the header matches the requested standard track, otherwise <see langword="null"/>.
        /// </summary>
        /// <param name="header">Header of the part</param>
        /// <param name="seekedHeader">Header to compare against</param>
        /// <param name="instrument">Instrument identity to provide the parser</param>
        /// <param name="difficulty">Difficulty identity to provide the parser</param>
        /// <param name="session">Session to provide the parser</param>
        private static StandardTrackParser? GetStandardTrackParser(string header, string seekedHeader, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingSession session) => header == seekedHeader ? new(difficulty, instrument, session) : null;

        /// <summary>
        /// Headers for standard tracks
        /// </summary>
        private static readonly Dictionary<string, (Difficulty, StandardInstrumentIdentity)> standardTrackHeaders = GetTrackCombinations(Enum.GetValues<StandardInstrumentIdentity>()).ToDictionary(tuple => ChartFormatting.Header((InstrumentIdentity)tuple.instrument, tuple.difficulty));

        /// <inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static Track<StandardChord> ReadTrack(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default)
        {
            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var reader = new ChartFileReader(path, header => GetStandardTrackParser(header, seekedHeader, instrument, difficulty, new(config ?? DefaultConfig)));

            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out StandardTrackParser? parser) ? parser!.Result! : new();
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, CancellationToken, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        /// <returns></returns>
        public static async Task<Track<StandardChord>> ReadTrackAsync(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            config ??= DefaultConfig;

            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var reader = new ChartFileReader(path, header => GetStandardTrackParser(header, seekedHeader, instrument, difficulty, new(config ?? DefaultConfig)));

            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out StandardTrackParser? parser) ? parser!.Result! : new();
        }
        #endregion
        #endregion
        #region Metadata
        private static MetadataParser? GetMetadataParser(string header, ReadingSession session) => header == ChartFormatting.MetadataHeader ? new(session) : null;
        /// <summary>
        /// Reads metadata from a chart file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        public static Metadata ReadMetadata(string path)
        {
            var reader = new ChartFileReader(path, header => GetMetadataParser(header, new(DefaultConfig)));
            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result! : new();
        }
        /// <summary>
        /// Reads metadata from a chart file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="ReadMetadata(string)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<Metadata> ReadMetadataAsync(string path, CancellationToken cancellationToken)
        {
            var reader = new ChartFileReader(path, header => GetMetadataParser(header, new(DefaultConfig)));
            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result! : new();
        }
        #endregion
        #region Global events
        /// <summary>
        /// Creates a <see cref="SyncTrackParser"/> if the header matches the sync track header, otherwise <see langword="null"/>.
        /// </summary>
        private static GlobalEventParser? GetGlobalEventParser(string header) => header == ChartFormatting.GlobalEventHeader ? new(new(DefaultConfig)) : null;

        /// <inheritdoc cref="GlobalEvent.FromFile(string)"/>
        /// <param name="path"><inheritdoc cref="GlobalEvent.FromFile(string)" path="/param[@name='path']"/></param>
        public static List<GlobalEvent> ReadGlobalEvents(string path)
        {
            var reader = new ChartFileReader(path, GetGlobalEventParser);
            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : new();
        }
        /// <inheritdoc cref="GlobalEvent.FromFileAsync(string, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="GlobalEvent.FromFileAsync(string, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="GlobalEvent.FromFileAsync(string, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns></returns>
        public static async Task<List<GlobalEvent>> ReadGlobalEventsAsync(string path, CancellationToken cancellationToken)
        {
            var reader = new ChartFileReader(path, GetGlobalEventParser);
            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : new();
        }

        /// <summary>
        /// Reads lyrics from a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="Phrase"/> containing the lyrics from the file</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadGlobalEvents(string)" path="/exception"/>
        public static IEnumerable<Phrase> ReadLyrics(string path) => ReadGlobalEvents(path).GetLyrics();
        /// <summary>
        /// Reads lyrics from a chart file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="ReadLyrics(string)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<IEnumerable<Phrase>> ReadLyricsAsync(string path, CancellationToken cancellationToken) => (await ReadGlobalEventsAsync(path, cancellationToken)).GetLyrics();
        #endregion
        #region Sync track
        /// <summary>
        /// Creates a <see cref="SyncTrackParser"/> if the header matches the sync track header, otherwise <see langword="null"/>.
        /// </summary>
        private static SyncTrackParser? GetSyncTrackParser(string header, ReadingSession session) => header == ChartFormatting.SyncTrackHeader ? new(session) : null;

        /// <inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="config"><inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static SyncTrack ReadSyncTrack(string path, ReadingConfiguration? config)
        {
            config ??= DefaultConfig;

            var reader = new ChartFileReader(path, (header) => GetSyncTrackParser(header, new(config ?? DefaultConfig)));
            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
        }
        /// <inheritdoc cref="SyncTrack.FromFileAsync(string, CancellationToken, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="SyncTrack.FromFileAsync(string, CancellationToken, ReadingConfiguration?)" path="/param[­@name='path']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="SyncTrack.FromFileAsync(string, CancellationToken, ReadingConfiguration?)" path="/param[­@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="SyncTrack.FromFileAsync(string, CancellationToken, ReadingConfiguration?)" path="/param[­@name='config']"/></param>
        public static async Task<SyncTrack> ReadSyncTrackAsync(string path, CancellationToken cancellationToken, ReadingConfiguration? config = default)
        {
            config ??= DefaultConfig;

            var reader = new ChartFileReader(path, (header) => GetSyncTrackParser(header, new(config ?? DefaultConfig)));
            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
        }
        #endregion

        /// <summary>
        /// Splits the data of an entry.
        /// </summary>
        /// <param name="data">Data portion of a <see cref="Entries.TrackObjectEntry"/></param>
        internal static string[] GetDataSplit(string data) => data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        /// <summary>
        /// Gets all the combinations of instruments and difficulties.
        /// </summary>
        /// <param name="instruments">Enum containing the instruments</param>
        private static IEnumerable<(Difficulty difficulty, TInstEnum instrument)> GetTrackCombinations<TInstEnum>(IEnumerable<TInstEnum> instruments) => from difficulty in Enum.GetValues<Difficulty>() from instrument in instruments select (difficulty, instrument);
    }
}
