using ChartTools.Events;
using ChartTools.Extensions;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Formatting;
using ChartTools.IO.Chart.Parsing;
using ChartTools.IO.Chart.Serializing;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Parsing;
using ChartTools.IO.Sections;
using ChartTools.Lyrics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChartTools.IO.Chart
{
    /// <summary>
    /// Provides methods for reading and writing chart files
    /// </summary>
    public static class ChartFile
    {
        /// <summary>
        /// Default configuration to use for reading when the provided configuration is <see langword="default"/>
        /// </summary>
        public static ReadingConfiguration DefaultReadConfig { get; set; } = new()
        {
            DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert
        };
        /// <summary>
        /// Default configuration to use for writing when the provided configuration is <see langword="default"/>
        /// </summary>
        public static WritingConfiguration DefaultWriteConfig { get; set; } = new()
        {
            SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
            EventSource = TrackObjectSource.Seperate,
            StarPowerSource = TrackObjectSource.Seperate,
            UnsupportedModifierPolicy = UnsupportedModifierPolicy.ThrowException
        };

        #region Reading
        #region Song
        /// <summary>
        /// Creates a <see cref="ChartParser"/> for parsing a section based on the header.
        /// </summary>
        /// <exception cref="FormatException"></exception>
        private static ChartParser GetSongParser(string header, ReadingSession session)
        {
            switch (header)
            {
                case ChartFormatting.MetadataHeader:
                    return new MetadataParser();
                case ChartFormatting.GlobalEventHeader:
                    return new GlobalEventParser(session);
                case ChartFormatting.SyncTrackHeader:
                    return new SyncTrackParser(session);
                default:
                    if (drumsTrackHeaders.TryGetValue(header, out Difficulty diff))
                        return new DrumsTrackParser(diff, session, header);
                    else if (ghlTrackHeaders.TryGetValue(header, out (Difficulty, GHLInstrumentIdentity) ghlTuple))
                        return new GHLTrackParser(ghlTuple.Item1, ghlTuple.Item2, session, header);
                    else if (standardTrackHeaders.TryGetValue(header, out (Difficulty, StandardInstrumentIdentity) standardTuple))
                        return new StandardTrackParser(standardTuple.Item1, standardTuple.Item2, session, header);
                    else
                    {
                        return session.Configuration.UnknownSectionPolicy == UnknownSectionPolicy.ThrowException
                            ? throw new Exception($"Unknown section with header \"{header}\". Consider using {UnknownSectionPolicy.Store} to avoid this error.")
                            : new UnknownSectionParser(session, header);
                    }
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
                parser.ApplyToSong(song);

            return song;
        }

        /// <inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)"/>
        /// <param name="path"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="config"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static Song ReadSong(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetSongParser(header, session));

            reader.Read();
            return CreateSongFromReader(reader);
        }

        /// <inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='path']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Song.FromFileAsync(string, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@='config']"/></param>
        public static async Task<Song> ReadSongAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetSongParser(header, session));

            await reader.ReadAsync(cancellationToken);
            return CreateSongFromReader(reader);
        }
        #endregion
        #region Instruments
        /// <summary>
        /// Combines the results from the parsers in a <see cref="ChartFileReader"/> into an instrument.
        /// </summary>
        private static Instrument<TChord>? CreateInstrumentFromReader<TChord>(ChartFileReader reader) where TChord : Chord, new()
        {
            Instrument<TChord>? output = null;

            foreach (TrackParser<TChord> parser in reader.Parsers)
                (output ??= new()).SetTrack(parser.Result!);

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
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <inheritdoc cref="ReadDrums(string, ReadingConfiguration, FormattingRules?)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, GHLInstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/exception"/>
        /// <inheritdoc cref="ReadInstrument(string, StandardInstrumentIdentity, ReadingConfiguration, FormattingRules?)" path="/exception"/>
        public static Instrument? ReadInstrument(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return ReadDrums(path, config, formatting);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return ReadInstrument(path, (GHLInstrumentIdentity)instrument, config, formatting);
            return Enum.IsDefined((StandardInstrumentIdentity)instrument)
                ? ReadInstrument(path, (StandardInstrumentIdentity)instrument, config, formatting)
                : throw new UndefinedEnumException(instrument);
        }
        public static async Task<Instrument?> ReadInstrumentAsync(string path, InstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return await ReadDrumsAsync(path, config, formatting, cancellationToken);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return await ReadInstrumentAsync(path, (GHLInstrumentIdentity)instrument, config, formatting, cancellationToken);
            return Enum.IsDefined((StandardInstrumentIdentity)instrument)
                ? await ReadInstrumentAsync(path, (StandardInstrumentIdentity)instrument, config, formatting, cancellationToken)
                : throw new UndefinedEnumException(instrument);
        }
        #region Drums
        private static DrumsTrackParser? GetAnyDrumsTrackParser(string header, ReadingSession session) => drumsTrackHeaders.TryGetValue(header, out Difficulty difficulty)
            ? new(difficulty, session, header)
            : null;
        /// <summary>
        /// Reads drums from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="DrumsChord"/> containing all drums data
        ///     <para><see langword="null"/> if the file contains no drums data</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
        /// <param name="formatting"><inheritdoc cref="FormattingRules" path="/summary"/></param>
        public static Instrument<DrumsChord>? ReadDrums(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetAnyDrumsTrackParser(header, session));

            reader.Read();
            return CreateInstrumentFromReader<DrumsChord>(reader);
        }
        public static async Task<Instrument<DrumsChord>?> ReadDrumsAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetAnyDrumsTrackParser(header, session));

            await reader.ReadAsync(cancellationToken);
            return CreateInstrumentFromReader<DrumsChord>(reader);
        }
        #endregion
        #region GHL
        private static GHLTrackParser? GetAnyGHLTrackParser(string header, GHLInstrumentIdentity instrument, ReadingSession session) => ghlTrackHeaders.TryGetValue(header, out (Difficulty, GHLInstrumentIdentity) tuple) && tuple.Item2 == instrument
            ? new(tuple.Item1, tuple.Item2, session, header)
            : null;
        /// <summary>
        /// Reads a Guitar Hero Live instrument from a chart file.
        /// </summary>
        /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="GHLChord"/> containing all data about the given instrument
        ///     <para><see langword="null"/> if the file has no data for the given instrument</para>
        /// </returns>
        /// <param name="path">Path of the file to read</param>
        /// <inheritdoc cref="GetGHLTrackParser(string, string, GHLInstrumentIdentity, Difficulty, ReadingSession)" path="/exception"/>
        public static Instrument<GHLChord>? ReadInstrument(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(instrument);

            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetAnyGHLTrackParser(header, instrument, session));

            reader.Read();
            return CreateInstrumentFromReader<GHLChord>(reader);
        }
        public static async Task<Instrument<GHLChord>?> ReadInstrumentAsync(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            Validator.ValidateEnum(instrument);

            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetAnyGHLTrackParser(header, instrument, session));

            await reader.ReadAsync(cancellationToken);
            return CreateInstrumentFromReader<GHLChord>(reader);
        }
        #endregion
        #region Standard
        private static StandardTrackParser? GetAnyStandardTrackParser(string header, StandardInstrumentIdentity instrument, ReadingSession session) => standardTrackHeaders.TryGetValue(header, out (Difficulty, StandardInstrumentIdentity) tuple) && tuple.Item2 == instrument
            ? new(tuple.Item1, tuple.Item2, session, header)
            : null;
        /// <inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?)"/>
        /// <param name="path"><inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='instrument']"/></param>
        /// <param name="config"><inheritdoc cref="Instrument.FromFile(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static Instrument<StandardChord>? ReadInstrument(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(instrument);

            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetAnyStandardTrackParser(header, instrument, session));

            reader.Read();
            return CreateInstrumentFromReader<StandardChord>(reader);
        }
        /// <inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='instrument']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity,  ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Instrument.FromFileAsync(string, StandardInstrumentIdentity, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='config']"/></param>
        public static async Task<Instrument<StandardChord>?> ReadInstrumentAsync(string path, StandardInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            Validator.ValidateEnum(instrument);

            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetAnyStandardTrackParser(header, instrument, session));

            await reader.ReadAsync(cancellationToken);
            return CreateInstrumentFromReader<StandardChord>(reader);
        }
        #endregion
        #endregion
        #region Tracks
        /// <inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFile(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static Track ReadTrack(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return ReadDrumsTrack(path, difficulty, config, formatting);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return ReadTrack(path, (GHLInstrumentIdentity)instrument, difficulty, config, formatting);
            if (Enum.IsDefined((StandardInstrumentIdentity)instrument))
                return ReadTrack(path, (StandardInstrumentIdentity)instrument, difficulty, config, formatting);

            throw new UndefinedEnumException(instrument);
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, InstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='config']"/></param>
        public static async Task<Track> ReadTrackAsync(string path, InstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            if (instrument == InstrumentIdentity.Drums)
                return await ReadDrumsTrackAsync(path, difficulty, config, formatting, cancellationToken);
            if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
                return await ReadTrackAsync(path, (GHLInstrumentIdentity)instrument, difficulty, config, formatting, cancellationToken);
            if (Enum.IsDefined((StandardInstrumentIdentity)instrument))
                return await ReadTrackAsync(path, (StandardInstrumentIdentity)instrument, difficulty, config, formatting, cancellationToken);

            throw new UndefinedEnumException(instrument);
        }
        #region Drums
        /// <summary>
        /// Creates a <see cref="DrumsTrackParser"/> is the header matches the requested standard track, otherwise <see langword="null"/>.
        /// </summary>
        /// <param name="header">Header of the part</param>
        /// <param name="seekedHeader">Header to compare against</param>
        /// <param name="difficulty">Difficulty identity to provide the parser</param>
        /// <param name="session">Session to provide the parser</param>
        private static DrumsTrackParser? GetDrumsTrackParser(string header, string seekedHeader, Difficulty difficulty, ReadingSession session) => header == seekedHeader ? new(difficulty, session, header) : null;
        /// <summary>
        /// Headers for drums tracks
        /// </summary>
        private static readonly Dictionary<string, Difficulty> drumsTrackHeaders = EnumCache<Difficulty>.Values.ToDictionary(diff => ChartFormatting.Header(ChartFormatting.DrumsHeaderName, diff));
        /// <inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?, FormattingRules?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFile(string, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static Track<DrumsChord> ReadDrumsTrack(string path, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(difficulty);

            var seekedHeader = ChartFormatting.Header(InstrumentIdentity.Drums, difficulty);
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetDrumsTrackParser(header, seekedHeader, difficulty, session));

            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out DrumsTrackParser? parser) ? parser!.Result! : new();
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='config']"/></param>
        public static async Task<Track<DrumsChord>> ReadDrumsTrackAsync(string path, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            Validator.ValidateEnum(difficulty);

            var seekedHeader = ChartFormatting.Header(ChartFormatting.DrumsHeaderName, difficulty);
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetDrumsTrackParser(header, seekedHeader, difficulty, session));

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
        private static GHLTrackParser? GetGHLTrackParser(string header, string seekedHeader, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingSession session) => header == seekedHeader ? new(difficulty, instrument, session, header) : null;
        /// <summary>
        /// Headers for GHL tracks
        /// </summary>
        private static readonly Dictionary<string, (Difficulty, GHLInstrumentIdentity)> ghlTrackHeaders = GetTrackCombinations(Enum.GetValues<GHLInstrumentIdentity>()).ToDictionary(tuple => ChartFormatting.Header(tuple.instrument, tuple.difficulty));
        /// <inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='config']"/></param>
        public static Track<GHLChord> ReadTrack(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(instrument);
            Validator.ValidateEnum(difficulty);

            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetGHLTrackParser(header, seekedHeader, instrument, difficulty, session));

            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out GHLTrackParser? parser) ? parser!.Result : new();
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, GHLInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='config']"/></param>
        public static async Task<Track<GHLChord>> ReadTrackAsync(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            Validator.ValidateEnum(instrument);
            Validator.ValidateEnum(difficulty);

            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetGHLTrackParser(header, seekedHeader, instrument, difficulty, session));

            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out GHLTrackParser? parser) ? parser!.Result : new();
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
        private static StandardTrackParser? GetStandardTrackParser(string header, string seekedHeader, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingSession session) => header == seekedHeader ? new(difficulty, instrument, session, header) : null;

        /// <summary>
        /// Headers for standard tracks
        /// </summary>
        private static readonly Dictionary<string, (Difficulty, StandardInstrumentIdentity)> standardTrackHeaders = GetTrackCombinations(Enum.GetValues<StandardInstrumentIdentity>()).ToDictionary(tuple => ChartFormatting.Header((InstrumentIdentity)tuple.instrument, tuple.difficulty));

        /// <inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='difficulty']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFile(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?)" path="/param[@name='config']"/></param>
        public static Track<StandardChord> ReadTrack(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default)
        {
            Validator.ValidateEnum(instrument);
            Validator.ValidateEnum(difficulty);

            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetStandardTrackParser(header, seekedHeader, instrument, difficulty, session));

            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out StandardTrackParser? parser) ? parser!.Result! : new();
        }
        /// <inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='path']"/></param>
        /// <param name="instrument"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='instrument']"/></param>
        /// <param name="difficulty"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='difficulty']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="Track.FromFileAsync(string, StandardInstrumentIdentity, Difficulty, ReadingConfiguration?, FormattingRules?, CancellationToken)" path="/param[@name='config']"/></param>
        /// <returns></returns>
        public static async Task<Track<StandardChord>> ReadTrackAsync(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            Validator.ValidateEnum(instrument);
            Validator.ValidateEnum(difficulty);

            var seekedHeader = ChartFormatting.Header(instrument, difficulty);
            var session = new ReadingSession(config ?? DefaultReadConfig, formatting ?? new());
            var reader = new ChartFileReader(path, header => GetStandardTrackParser(header, seekedHeader, instrument, difficulty, session));

            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out StandardTrackParser? parser) ? parser!.Result! : new();
        }
        #endregion
        #endregion
        #region Metadata
        private static MetadataParser? GetMetadataParser(string header) => header == ChartFormatting.MetadataHeader ? new() : null;
        /// <summary>
        /// Reads metadata from a chart file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        public static Metadata ReadMetadata(string path)
        {
            var reader = new ChartFileReader(path, header => GetMetadataParser(header));
            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
        }
        #endregion
        #region Global events
        /// <summary>
        /// Creates a <see cref="SyncTrackParser"/> if the header matches the sync track header, otherwise <see langword="null"/>.
        /// </summary>
        private static GlobalEventParser? GetGlobalEventParser(string header) => header == ChartFormatting.GlobalEventHeader ? new(null!) : null;

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
        public static async Task<List<GlobalEvent>> ReadGlobalEventsAsync(string path, CancellationToken cancellationToken = default)
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
        public static async Task<IEnumerable<Phrase>> ReadLyricsAsync(string path, CancellationToken cancellationToken = default) => (await ReadGlobalEventsAsync(path, cancellationToken)).GetLyrics();
        #endregion
        #region Sync track
        /// <summary>
        /// Creates a <see cref="SyncTrackParser"/> if the header matches the sync track header, otherwise <see langword="null"/>.
        /// </summary>
        private static SyncTrackParser? GetSyncTrackParser(string header, ReadingSession session) => header == ChartFormatting.SyncTrackHeader ? new(session) : null;

        /// <inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)"/>
        /// <param name="path"><inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
        /// <param name="config"><inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
        public static SyncTrack ReadSyncTrack(string path, ReadingConfiguration? config, FormattingRules? formatting = default)
        {
            config ??= DefaultReadConfig;

            var reader = new ChartFileReader(path, (header) => GetSyncTrackParser(header, new(config ?? DefaultReadConfig, formatting ?? new())));
            reader.Read();
            return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
        }
        /// <inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)"/>
        /// <param name="path"><inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)" path="/param[­@name='path']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)" path="/param[­@name='cancellationToken']"/></param>
        /// <param name="config"><inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)" path="/param[­@name='config']"/></param>
        public static async Task<SyncTrack> ReadSyncTrackAsync(string path, ReadingConfiguration? config = default, CancellationToken cancellationToken = default)
        {
            config ??= DefaultReadConfig;

            var reader = new ChartFileReader(path, (header) => GetSyncTrackParser(header, new(config ?? DefaultReadConfig, null)));
            await reader.ReadAsync(cancellationToken);
            return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
        }
        #endregion
        #endregion

        #region Writing
        /// <summary>
        /// Writes a song to a chart file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="song">Song to write</param>
        public static void WriteSong(string path, Song song, WritingConfiguration? config = default)
        {
            var writer = GetSongWriter(path, song, new(config ?? DefaultWriteConfig, song.Metadata.Formatting));
            writer.Write();
        }
        public static async Task WriteSongAsync(string path, Song song, WritingConfiguration? config = default, CancellationToken cancellationToken = default)
        {
            var writer = GetSongWriter(path, song, new(config ?? DefaultWriteConfig, song.Metadata.Formatting));
            await writer.WriteAsync(cancellationToken);
        }
        private static ChartFileWriter GetSongWriter(string path, Song song, WritingSession session)
        {
            var instruments = song.Instruments.NonNull().ToArray();
            var serializers = new List<Serializer<string>>(instruments.Length + 2);
            var removedHeaders = new List<string>();

            serializers.Add(new MetadataSerializer(song.Metadata));

            if (!song.SyncTrack.IsEmpty)
                serializers.Add(new SyncTrackSerializer(song.SyncTrack, session));
            else
                removedHeaders.Add(ChartFormatting.SyncTrackHeader);

            if (song.GlobalEvents.Count > 0)
                serializers.Add(new GlobalEventSerializer(song.GlobalEvents, session));
            else
                removedHeaders.Add(ChartFormatting.GlobalEventHeader);

            var difficulties = EnumCache<Difficulty>.Values;

            // Remove headers for null instruments
            removedHeaders.AddRange((from identity in Enum.GetValues<InstrumentIdentity>()
                                     where instruments.Any(instrument => instrument.InstrumentIdentity == identity)
                                     let instrumentName = ChartFormatting.InstrumentHeaderNames[identity]
                                     let headers = from diff in difficulties
                                                   select ChartFormatting.Header(identity, diff)
                                     select headers).SelectMany(h => h));

            foreach (var instrument in instruments)
            {
                var instrumentName = ChartFormatting.InstrumentHeaderNames[instrument.InstrumentIdentity];
                var tracks = instrument.GetExistingTracks().ToArray();

                serializers.AddRange(tracks.Select(t => new TrackSerializer(t, session)));
                removedHeaders.AddRange(difficulties.Where(diff => !tracks.Any(t => t.Difficulty == diff)).Select(diff => ChartFormatting.Header(instrumentName, diff)));
            }

            if (song.UnknownChartSections is not null)
                serializers.AddRange(song.UnknownChartSections.Select(s => new UnknownSectionSerializer(s.Header, s, session)));

            return new(path, removedHeaders, serializers.ToArray());
        }

        /// <summary>
        /// Replaces an instrument in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        public static void ReplaceInstrument(string path, Instrument instrument, WritingConfiguration? config = default, FormattingRules? formatting = default)
        {
            var writer = GetInstrumentWriter(path, instrument, new(config ?? DefaultWriteConfig, formatting ?? new()));
            writer.Write();
        }
        public static async Task ReplaceInstrumentAsync(string path, Instrument instrument, WritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            var writer = GetInstrumentWriter(path, instrument, new(config ?? DefaultWriteConfig, formatting ?? new()));
            await writer.WriteAsync(cancellationToken);
        }
        private static ChartFileWriter GetInstrumentWriter(string path, Instrument instrument, WritingSession session)
        {
            if (!Enum.IsDefined(instrument.InstrumentIdentity))
                throw new ArgumentException("Instrument cannot be written because its identity is unknown.", nameof(instrument));

            var instrumentName = ChartFormatting.InstrumentHeaderNames[instrument.InstrumentIdentity];
            var tracks = instrument.GetExistingTracks().ToArray();

            return new(path,
                EnumCache<Difficulty>.Values.Where(d => !tracks.Any(t => t.Difficulty == d)).Select(d => ChartFormatting.Header(instrumentName, d)),
                tracks.Select(t => new TrackSerializer(t, session)).ToArray());
        }

        public static void ReplaceTrack(string path, Track track, WritingConfiguration? config = default, FormattingRules? formatting = default)
        {
            var writer = GetTrackWriter(path, track, new(config ?? DefaultWriteConfig, formatting ?? new()));
            writer.Write();
        }
        public static async Task ReplaceTrackAsync(string path, Track track, WritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        {
            var writer = GetTrackWriter(path, track, new(config ?? DefaultWriteConfig, formatting ?? new()));
            await writer.WriteAsync(cancellationToken);
        }
        private static ChartFileWriter GetTrackWriter(string path, Track track, WritingSession session)
        {
            if (track.ParentInstrument is null)
                throw new ArgumentNullException(nameof(track), "Cannot write track because it does not belong to an instrument.");
            if (!Enum.IsDefined(track.ParentInstrument.InstrumentIdentity))
                throw new ArgumentException("Cannot write track because the instrument it belongs to is unknown.", nameof(track));

            return new(path, null, new TrackSerializer(track, session));
        }

        /// <summary>
        /// Replaces the metadata in a file.
        /// </summary>
        /// <param name="path">Path of the file to read</param>
        /// <param name="metadata">Metadata to write</param>
        public static void ReplaceMetadata(string path, Metadata metadata)
        {
            var writer = GetMetadataWriter(path, metadata);
            writer.Write();
        }
        private static ChartFileWriter GetMetadataWriter(string path, Metadata metadata) => new(path, null, new MetadataSerializer(metadata));

        /// <summary>
        /// Replaces the global events in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="events">Events to use as a replacement</param>
        public static void ReplaceGlobalEvents(string path, IEnumerable<GlobalEvent> events)
        {
            var writer = GetGlobalEventWriter(path, events, new(DefaultWriteConfig, null));
            writer.Write();
        }
        public static async Task ReplaceGlobalEventsAsync(string path, IEnumerable<GlobalEvent> events, CancellationToken cancellationToken = default)
        {
            var writer = GetGlobalEventWriter(path, events, new(DefaultWriteConfig, null));
            await writer.WriteAsync(cancellationToken);
        }
        private static ChartFileWriter GetGlobalEventWriter(string path, IEnumerable<GlobalEvent> events, WritingSession session) => new(path, null, new GlobalEventSerializer(events, session));

        /// <summary>
        /// Replaces the sync track in a file.
        /// </summary>
        /// <param name="path">Path of the file to write</param>
        /// <param name="syncTrack">Sync track to write</param>
        public static void ReplaceSyncTrack(string path, SyncTrack syncTrack, WritingConfiguration? config = default)
        {
            var writer = GetSyncTrackWriter(path, syncTrack, new(config ?? DefaultWriteConfig, null));
            writer.Write();
        }
        public static async Task ReplaceSyncTrackAsync(string path, SyncTrack syncTrack, WritingConfiguration? config = default, CancellationToken cancellationToken = default)
        {
            var writer = GetSyncTrackWriter(path, syncTrack, new(config ?? DefaultWriteConfig, null));
            await writer.WriteAsync(cancellationToken);
        }
        private static ChartFileWriter GetSyncTrackWriter(string path, SyncTrack syncTrack, WritingSession session) => new(path, null, new SyncTrackSerializer(syncTrack, session));
        #endregion

        /// <summary>
        /// Gets all the combinations of instruments and difficulties.
        /// </summary>
        /// <param name="instruments">Enum containing the instruments</param>
        private static IEnumerable<(Difficulty difficulty, TInstEnum instrument)> GetTrackCombinations<TInstEnum>(IEnumerable<TInstEnum> instruments) => from difficulty in EnumCache<Difficulty>.Values from instrument in instruments select (difficulty, instrument);
    }
}
