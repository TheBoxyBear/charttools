using ChartTools.IO.Chart.Parsers;
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
    public static partial class ChartParser
    {
        public static ReadingConfiguration DefaultReadConfig = new()
        {
            DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
        };

        private static readonly Dictionary<string, (Difficulty, Instruments)> trackHeaders = GetTrackCombinations(Enum.GetValues<Instruments>().Except(new Instruments[] { Instruments.Vocals })).ToDictionary(tuple => CreateHeader(tuple.instrument, tuple.difficulty));

        #region Song
        private static ChartPartParser? GetSongParser(string part)
        {
            switch (part)
            {
                case "[Song]":
                    return new MetadataParser();
                case "[Events]":
                    return new GlobalEventParser();
                case "[SyncTrack]":
                    return new SyncTrackParser();
                default:
                    if (drumsTrackHeaders.TryGetValue(part, out Difficulty diff))
                        return new DrumsTrackParser(diff);
                    else if (ghlTrackHeaders.TryGetValue(part, out (Difficulty, GHLInstrument) standardTuple))
                        return new GHLTrackParser(standardTuple.Item1, standardTuple.Item2);
                    else if (standardTrackHeaders.TryGetValue(part, out (Difficulty, StandardInstrument) ghlTuple))
                        return new StandardTrackParser(ghlTuple.Item1, ghlTuple.Item2);
                    else
                        throw new FormatException($"Unknown part: {part}"); // TODO Add support for unknown parts in configuration
            }
        }

        private static Song CreateSongFromReader(ChartReader reader)
        {
            Song song = new();

            foreach (var parser in reader.Parsers)
                parser.ApplyResultToSong(song);

            return song;
        }

        public static Song ReadSong(string path, ReadingConfiguration? config)
        {
            var reader = new ChartReader(path, GetSongParser);
            reader.Read(new(config));
            return CreateSongFromReader(reader);
        }
        /// <summary>
        /// Reads a chart file asynchronously using threads to parse parts.
        /// </summary>
        /// <returns>Instance of <see cref="Song"/> containing all song data</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        public static async Task<Song> ReadSongAsync(string path, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetSongParser);
            await reader.ReadAsync(new(config), cancellationToken);
            return CreateSongFromReader(reader);
        }
        #endregion
        #region Instruments
        private static Instrument<TChord>? CreateInstrumentFromReader<TChord>(ChartReader reader) where TChord : Chord
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
        /// <inheritdoc cref="ReadInstrument(string, GHLInstrument, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, StandardInstrument, ReadingConfiguration)" path="/exception"/>
        public static Instrument? ReadInstrument(string path, Instruments instrument, ReadingConfiguration config)
        {
            if (instrument == Instruments.Drums)
                return ReadDrums(path, config);
            if (Enum.IsDefined((GHLInstrument)instrument))
                return ReadInstrument(path, (GHLInstrument)instrument, config);
            return Enum.IsDefined((StandardInstrument)instrument)
                ? ReadInstrument(path, (StandardInstrument)instrument, config)
                : throw CommonExceptions.GetUndefinedException(instrument);
        }
        public static async Task<Instrument?> ReadInstrument(string path, Instruments instrument, ReadingConfiguration config, CancellationToken cancellationToken)
        {
            if (instrument == Instruments.Drums)
                return await ReadDrumsAsync(path, config, cancellationToken);
            if (Enum.IsDefined((GHLInstrument)instrument))
                return await ReadInstrumentAsync(path, (GHLInstrument)instrument, config, cancellationToken);
            return Enum.IsDefined((StandardInstrument)instrument)
                ? await ReadInstrumentAsync(path, (StandardInstrument)instrument, config, cancellationToken)
                : throw CommonExceptions.GetUndefinedException(instrument);
        }
        #region Drums
        private static DrumsTrackParser? GetAnyDrumsTrackParser(string part) => drumsTrackHeaders.TryGetValue(part, out Difficulty difficulty)
            ? new(difficulty)
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
        public static Instrument<DrumsChord>? ReadDrums(string path, ReadingConfiguration? config)
        {
            var reader = new ChartReader(path, GetAnyDrumsTrackParser);
            reader.Read(new(config));
            return CreateInstrumentFromReader<DrumsChord>(reader);
        }
        public static async Task<Instrument<DrumsChord>?> ReadDrumsAsync(string path, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetAnyDrumsTrackParser);
            await reader.ReadAsync(new(config), cancellationToken);
            return CreateInstrumentFromReader<DrumsChord>(reader);
        }
        #endregion
        #region GHL
        private static GHLTrackParser? GetAnyGHLTrackParser(string part) => ghlTrackHeaders.TryGetValue(part, out (Difficulty, GHLInstrument) tuple)
            ? new(tuple.Item1, tuple.Item2)
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
        public static Instrument<GHLChord>? ReadInstrument(string path, GHLInstrument instrument, ReadingConfiguration? config)
        {
            var reader = new ChartReader(path, GetAnyStandardTrackParser);
            reader.Read(new(config));
            return CreateInstrumentFromReader<GHLChord>(reader);
        }
        public static async Task<Instrument<GHLChord>?> ReadInstrumentAsync(string path, GHLInstrument instrument, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetAnyStandardTrackParser);
            await reader.ReadAsync(new(config), cancellationToken);
            return CreateInstrumentFromReader<GHLChord>(reader);
        }
        #endregion
        #region Standard
        private static StandardTrackParser? GetAnyStandardTrackParser(string part) => standardTrackHeaders.TryGetValue(part, out (Difficulty, StandardInstrument) tuple)
            ? new(tuple.Item1, tuple.Item2)
            : null;
        /// <summary>
        /// Reads a standard instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="StandardChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file contains no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument to read</param>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        /// <inheritdoc cref="GetStandardTrack(IEnumerable{string}, ReadingConfiguration)" path="/exception"/>
        public static Instrument<StandardChord>? ReadInstrument(string path, StandardInstrument instrument, ReadingConfiguration? config)
        {
            var reader = new ChartReader(path, GetAnyStandardTrackParser);
            reader.Read(new(config));
            return CreateInstrumentFromReader<StandardChord>(reader);
        }
        public static async Task<Instrument<StandardChord>?> ReadInstrumentAsync(string path, StandardInstrument instrument, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetAnyStandardTrackParser);
            await reader.ReadAsync(new(config), cancellationToken);
            return CreateInstrumentFromReader<StandardChord>(reader);
        }
        #endregion
        #endregion
        #region Tracks
        /// <summary>
        /// Reads a track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track"/> containing all data about the given track
        ///     <para><see langword="null"/> if the file contains no data for the given track</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument of the track to read</param>
        /// <param name="difficulty">Difficulty of the track to read</param>
        /// <inheritdoc cref="ReadDrumsTrack(string, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadTrack(string, GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="ReadTrack(string, StandardInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        public static Track? ReadTrack(string path, Instruments instrument, Difficulty difficulty, ReadingConfiguration? config)
        {
            if (instrument == Instruments.Drums)
                return ReadDrumsTrack(path, difficulty, config);
            if (Enum.IsDefined((GHLInstrument)instrument))
                return ReadTrack(path, (GHLInstrument)instrument, difficulty, config);
            if (Enum.IsDefined((StandardInstrument)instrument))
                return ReadTrack(path, (StandardInstrument)instrument, difficulty, config);

            throw CommonExceptions.GetUndefinedException(instrument);
        }
        public static async Task<Track?> ReadTrackAsync(string path, Instruments instrument, Difficulty difficulty, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            if (instrument == Instruments.Drums)
                return await ReadDrumsTrackAsync(path, difficulty, config, cancellationToken);
            if (Enum.IsDefined((GHLInstrument)instrument))
                return await ReadTrackAsync(path, (GHLInstrument)instrument, difficulty, config, cancellationToken);
            if (Enum.IsDefined((StandardInstrument)instrument))
                return await ReadTrackAsync(path, (StandardInstrument)instrument, difficulty, config, cancellationToken);

            throw CommonExceptions.GetUndefinedException(instrument);
        }
        #region Drums
        private static DrumsTrackParser? GetDrumsTrackParser(string header, string seekedHeader, Difficulty difficulty) => header == seekedHeader ? new(difficulty) : null;
        private static readonly Dictionary<string, Difficulty> drumsTrackHeaders = Enum.GetValues<Difficulty>().ToDictionary(diff => CreateHeader(DrumsHeaderName, diff));
        /// <summary>
        /// Reads a drums track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="DrumsChord"/> containing all drums data for the given difficulty
        ///     <para><see langword="null"/> if the file contains no drums data for the given difficulty</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="difficulty">Difficulty of the track to read</param>
        /// <inheritdoc cref="GetDrumsTrack(IEnumerable{string}, Difficulty, ReadingConfiguration), GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        public static Track<DrumsChord>? ReadDrumsTrack(string path, Difficulty difficulty, ReadingConfiguration? config)
        {
            var seekedHeader = CreateHeader(DrumsHeaderName, difficulty);
            var reader = new ChartReader(path, header => GetDrumsTrackParser(header, seekedHeader, difficulty));

            reader.Read(new(config));
            return reader.Parsers.TryGetFirstOfType(out DrumsTrackParser? parser) ? parser!.Result : null;
        }
        public static async Task<Track<DrumsChord>?> ReadDrumsTrackAsync(string path, Difficulty difficulty, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetAnyDrumsTrackParser);
            await reader.ReadAsync(new(config), cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out DrumsTrackParser? parser) ? parser!.Result : null;
        }
        #endregion
        #region GHL
        private static GHLTrackParser? GetGHLTrackParser(string header, string seekedHeader, GHLInstrument instrument, Difficulty difficulty) => header == seekedHeader ? new(difficulty, instrument) : null;
        private static readonly Dictionary<string, (Difficulty, GHLInstrument)> ghlTrackHeaders = GetTrackCombinations(Enum.GetValues<GHLInstrument>()).ToDictionary(tuple => CreateHeader(tuple.instrument, tuple.difficulty));
        /// <summary>
        /// Reads a Guitar Hero Live track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="GHLChord"/> containing all data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the file contains no data for the given instrument and difficulty</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetGHLTrack(IEnumerable{string}, GHLInstrument, Difficulty, ReadingConfiguration)" path="/exception"/>
        /// <inheritdoc cref="GetPart(IEnumerable{string}, string)" path="/exception"/>
        /// <inheritdoc cref="GetFullPartName(Instruments, Difficulty)(IEnumerable{string}, string)" path="/exception"/>
        public static Track<GHLChord>? ReadTrack(string path, GHLInstrument instrument, Difficulty difficulty, ReadingConfiguration? config)
        {
            var seekedHeder = CreateHeader(instrument, difficulty);
            var reader = new ChartReader(path, p => p == seekedHeder ? new GHLTrackParser(difficulty, instrument) : null);
            reader.Read(new(config));
            return reader.Parsers.TryGetFirstOfType(out GHLTrackParser? parser) ? parser!.Result : null;
        }
        public static async Task<Track<GHLChord>?> ReadTrackAsync(string path, GHLInstrument instrument, Difficulty difficulty, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var seekedHeader = CreateHeader(instrument, difficulty);
            var reader = new ChartReader(path, header => GetGHLTrackParser(header, seekedHeader, instrument, difficulty));

            await reader.ReadAsync(new(config), cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out GHLTrackParser? parser) ? parser!.Result : null;
        }
        #endregion
        #region Standard
        private static StandardTrackParser? GetStandardTrackParser(string header, string seekedHeader, StandardInstrument instrument, Difficulty difficulty) => header == seekedHeader ? new(difficulty, instrument) : null;
        private static readonly Dictionary<string, (Difficulty, StandardInstrument)> standardTrackHeaders = GetTrackCombinations(Enum.GetValues<StandardInstrument>()).ToDictionary(tuple => CreateHeader((Instruments)tuple.instrument, tuple.difficulty));

        /// <summary>
        /// Reads a standard track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Track{TChord}"/> where TChors is <see cref="StandardChord"/> containing all drums data for the given instrument and difficulty
        ///     <para><see langword="null"/> if the file contains no data for the given instrument and difficulty</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="instrument">Instrument of the track</param>
        /// <param name="difficulty">Difficulty of the track</param>
        /// <inheritdoc cref="GetStandardTrack(IEnumerable{string}, StandardInstrument, Difficulty, ReadingConfiguration)"/>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        public static Track<StandardChord>? ReadTrack(string path, StandardInstrument instrument, Difficulty difficulty, ReadingConfiguration? config)
        {
            var seekedHeader = CreateHeader(instrument, difficulty);
            var reader = new ChartReader(path, header => GetStandardTrackParser(header, seekedHeader, instrument, difficulty));

            reader.Read(new(config));
            return reader.Parsers.TryGetFirstOfType(out StandardTrackParser? parser) ? parser!.Result : null;
        }
        public static async Task<Track<StandardChord>?> ReadTrackAsync(string path, StandardInstrument instrument, Difficulty difficulty, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var seekedHeader = CreateHeader(instrument, difficulty);
            var reader = new ChartReader(path, header => GetStandardTrackParser(header, seekedHeader, instrument, difficulty));

            await reader.ReadAsync(new(config), cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out StandardTrackParser? parser) ? parser!.Result : null;
        }
        #endregion
        #endregion
        #region Metadata
        private static MetadataParser? GetMetadataParser(string part) => part == "[Song]" ? new() : null;
        /// <summary>
        /// Reads the metadata from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Metadata"/> containing metadata from the file
        ///     <para>Null if the file contains no metadata</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="GetMetadata(string[])" path="/exception"/>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        public static Metadata? ReadMetadata(string path)
        {
            var reader = new ChartReader(path, GetMetadataParser);
            reader.Read(new(DefaultReadConfig));
            return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : null;
        }
        public static async Task<Metadata?> ReadMetadataAsync(string path, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetMetadataParser);
            await reader.ReadAsync(new(DefaultReadConfig), cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : null;
        }
        #endregion
        #region Global events
        private static GlobalEventParser? GetGlobalEventParser(string part) => part == "[Events]" ? new() : null;

        /// <summary>
        /// Reads the global events from a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        /// <param name="path">Path of the file the read</param>
        /// <inheritdoc cref="GetGlobalEvents(string[])" path="/exception"/>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        public static List<GlobalEvent> ReadGlobalEvents(string path)
        {
            var reader = new ChartReader(path, GetGlobalEventParser);
            reader.Read(new(DefaultReadConfig));
            return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : new();
        }
        public static async Task<List<GlobalEvent>> ReadGlobalEventsAsync(string path, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetGlobalEventParser);
            await reader.ReadAsync(new(DefaultReadConfig), cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : new();
        }

        /// <summary>
        /// Reads the lyrics from a chart file.
        /// </summary>
        /// <returns>Enumerable of <see cref="Phrase"/> containing the lyrics from the file</returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="ReadGlobalEvents(string)(string[])" path="/exception"/>
        public static IEnumerable<Phrase> ReadLyrics(string path) => ReadGlobalEvents(path).GetLyrics();
        private static async Task<IEnumerable<Phrase>> ReadLyricsAsync(string path, CancellationToken cancellationToken) => (await ReadGlobalEventsAsync(path, cancellationToken)).GetLyrics();
        #endregion
        #region Sync track
        private static SyncTrackParser? GetSyncTrackParser(string part) => part == "[SyncTrack]" ? new() : null;

        /// <summary>
        /// Reads the sync track from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="SyncTrack"/>
        ///     <para><see langword="null"/> if the file contains no sync track</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="GetSyncTrack(string[])" path="/exception"/>
        /// <inheritdoc cref="ReadFileAsync(string)" path="/exception"/>
        public static SyncTrack? ReadSyncTrack(string path, ReadingConfiguration? config)
        {
            var reader = new ChartReader(path, GetSyncTrackParser);
            reader.Read(new(config));
            return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result : null;
        }
        public static async Task<SyncTrack?> ReadSyncTrackAsync(string path, ReadingConfiguration? config, CancellationToken cancellationToken)
        {
            var reader = new ChartReader(path, GetSyncTrackParser);
            await reader.ReadAsync(new(config), cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result : null;
        }
        #endregion

        /// <summary>
        /// Splits the data of an entry.
        /// </summary>
        /// <param name="data">Data portion of a <see cref="TrackObjectEntry"/></param>
        internal static string[] GetDataSplit(string data) => data.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        /// <summary>
        /// Generates an exception to throw when a line cannot be converted.
        /// </summary>
        /// <returns>Instance of <see cref="Exception"/> to throw</returns>
        /// <param name="line">Line that caused the exception</param>
        /// <param name="innerException">Exception caught when interpreting the line</param>
        internal static Exception GetLineException(string line, Exception innerException) => new FormatException($"Line \"{line}\": {innerException.Message}", innerException);

        private static IEnumerable<(Difficulty difficulty, TInstEnum instrument)> GetTrackCombinations<TInstEnum>(IEnumerable<TInstEnum> instruments) => from difficulty in Enum.GetValues<Difficulty>() from instrument in instruments select (difficulty, instrument);
    }
}
