using ChartTools.Events;
using ChartTools.Extensions;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Parsing;
using ChartTools.IO.Chart.Serializing;
using ChartTools.IO.Components;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.IO.Sources;
using ChartTools.Lyrics;

namespace ChartTools.IO.Chart;

/// <summary>
/// Provides methods for reading and writing chart files
/// </summary>
public static class ChartFile
{
    /// <summary>
    /// Default configuration to use for reading when the provided configuration is <see langword="default"/>
    /// </summary>
    public static ChartReadingConfiguration DefaultReadConfig { get; set; } = new()
    {
        DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
        OverlappingStarPowerPolicy = OverlappingSpecialPhrasePolicy.ThrowException,
        SnappedNotesPolicy = SnappedNotesPolicy.ThrowException,
        SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
        TempolessAnchorPolicy = TempolessAnchorPolicy.ThrowException,
        UnknownSectionPolicy = UnknownSectionPolicy.ThrowException
    };

    /// <summary>
    /// Default configuration to use for writing when the provided configuration is <see langword="default"/>
    /// </summary>
    public static ChartWritingConfiguration DefaultWriteConfig { get; set; } = new()
    {
        DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
        OverlappingStarPowerPolicy = OverlappingSpecialPhrasePolicy.ThrowException,
        SoloNoStarPowerPolicy = SoloNoStarPowerPolicy.Convert,
        SnappedNotesPolicy = SnappedNotesPolicy.ThrowException,
        UnsupportedModifierPolicy = UnsupportedModifierPolicy.ThrowException
    };

    #region Reading
    #region Song
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
    public static Song ReadSong(string path, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadSong(new ReadingDataSource(path), config, formatting);

    public static Song ReadSong(Stream stream, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadSong(new ReadingDataSource(stream), config, formatting);

    public static Song ReadSong(ReadingDataSource source, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    {
        var session = new ChartReadingSession(ComponentList.Full(), config, formatting);
        var reader = new ChartFileReader(source, session);

        reader.Read();
        return CreateSongFromReader(reader);
    }

    public static Task<Song> ReadSongAsync(string path, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => ReadSongAsync(new ReadingDataSource(path), config, formatting, cancellationToken);

    public static Task<Song> ReadSongAsync(Stream stream, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => ReadSongAsync(new ReadingDataSource(stream), config, formatting, cancellationToken);

    public static async Task<Song> ReadSongAsync(ReadingDataSource source, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        var session = new ChartReadingSession(ComponentList.Full(), config, formatting);
        var reader = new ChartFileReader(source, session);

        await reader.ReadAsync(cancellationToken);
        return CreateSongFromReader(reader);
    }

    public static Song ReadComponents(string path, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadComponents(new ReadingDataSource(path), components, config, formatting);

    public static Song ReadComponents(Stream stream, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    => ReadComponents(new ReadingDataSource(stream), components, config, formatting);

    public static Song ReadComponents(ReadingDataSource source, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    {
        var session = new ChartReadingSession(components, config, formatting);
        var reader = new ChartFileReader(source, session);

        reader.Read();
        return CreateSongFromReader(reader);
    }

    public static Task<Song> ReadComponentsAsync(string path, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => ReadComponentsAsync(new ReadingDataSource(path), components, config, formatting, cancellationToken);

    public static Task<Song> ReadComponentsAsync(Stream stream, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    => ReadComponentsAsync(new ReadingDataSource(stream), components, config, formatting, cancellationToken);

    public static async Task<Song> ReadComponentsAsync(ReadingDataSource source, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        var session = new ChartReadingSession(components, config, formatting);
        var reader = new ChartFileReader(source, session);

        await reader.ReadAsync(cancellationToken);
        return CreateSongFromReader(reader);
    }
    #endregion

    #region Instruments
    private static InstrumentSet CreateInstrumentSetFromReader(ChartFileReader reader)
    {
        var instruments = new InstrumentSet();

        foreach (var parser in reader.Parsers)
            switch (parser)
            {
                case DrumsTrackParser drumsParser:
                    instruments.Drums ??= new();
                    drumsParser.ApplyToInstrument(instruments.Drums);
                    break;
                case StandardTrackParser standardParser:
                    var standardInst = instruments.Get(standardParser.Instrument);

                    if (standardInst is null)
                    {
                        standardInst = new StandardInstrument(standardParser.Instrument);
                        instruments.Set(standardInst);
                    }

                    standardParser.ApplyToInstrument(standardInst);
                    break;
                case GHLTrackParser ghlParser:
                    var ghlInst = instruments.Get(ghlParser.Instrument);

                    if (ghlInst is null)
                    {
                        ghlInst = new GHLInstrument(ghlParser.Instrument);
                        instruments.Set(ghlInst);
                    }

                    ghlParser.ApplyToInstrument(ghlInst);
                    break;
            }

        return instruments;
    }

    public static InstrumentSet ReadInstruments(string path, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstruments(new ReadingDataSource(path), components, config, formatting);

    public static InstrumentSet ReadInstruments(Stream stream, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    => ReadInstruments(new ReadingDataSource(stream), components, config, formatting);

    public static InstrumentSet ReadInstruments(ReadingDataSource source, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    {
        var session = new ChartReadingSession(new() { Instruments = components }, config, formatting);
        var reader = new ChartFileReader(source, session);

        reader.Read();
        return CreateInstrumentSetFromReader(reader);
    }

    public static Task<InstrumentSet> ReadInstrumentsAsync(string path, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => ReadInstrumentsAsync(new ReadingDataSource(path), components, config, formatting, cancellationToken);

    public static Task<InstrumentSet> ReadInstrumentsAsync(Stream stream, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    => ReadInstrumentsAsync(new ReadingDataSource(stream), components, config, formatting, cancellationToken);

    public static async Task<InstrumentSet> ReadInstrumentsAsync(ReadingDataSource source, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        var session = new ChartReadingSession(new() { Instruments = components }, config, formatting);
        var reader = new ChartFileReader(source, session);

        await reader.ReadAsync(cancellationToken);
        return CreateInstrumentSetFromReader(reader);
    }

    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static Instrument? ReadInstrument(string path, InstrumentIdentity instrument, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    {
        if (instrument == InstrumentIdentity.Drums)
            return ReadDrums(path, difficulties, config, formatting);
        if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
            return ReadInstrument(path, (GHLInstrumentIdentity)instrument, difficulties, config, formatting);
        return Enum.IsDefined((StandardInstrumentIdentity)instrument)
            ? ReadInstrument(path, (StandardInstrumentIdentity)instrument, difficulties, config, formatting)
            : throw new UndefinedEnumException(instrument);
    }

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<Instrument?> ReadInstrumentAsync(string path, InstrumentIdentity instrument, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        if (instrument == InstrumentIdentity.Drums)
            return await ReadDrumsAsync(path, difficulties, config, formatting, cancellationToken);
        if (Enum.IsDefined((GHLInstrumentIdentity)instrument))
            return await ReadInstrumentAsync(path, (GHLInstrumentIdentity)instrument, difficulties, config, formatting, cancellationToken);
        return Enum.IsDefined((StandardInstrumentIdentity)instrument)
            ? await ReadInstrumentAsync(path, (StandardInstrumentIdentity)instrument, difficulties, config, formatting, cancellationToken)
            : throw new UndefinedEnumException(instrument);
    }

    #region Vocals
    /// <summary>
    /// Reads vocals from the global events in a chart file.
    /// </summary>
    /// <returns>Instance of <see cref="Instrument{TChord}"/> where TChord is <see cref="Phrase"/> containing lyric and timing data
    ///     <para><see langword="null"/> if the file contains no drums data</para>
    /// </returns>
    /// <param name="path">Path of the file to read</param>
    public static Vocals? ReadVocals(string path) => BuildVocals(ReadGlobalEvents(path));

    public static async Task<Vocals?> ReadVocalsAsync(string path, CancellationToken cancellationToken = default) => BuildVocals(await ReadGlobalEventsAsync(path, cancellationToken));

    private static Vocals? BuildVocals(IList<GlobalEvent> events)
    {
        var lyrics = events.GetLyrics().ToArray();

        if (lyrics.Length == 0)
            return null;

        var instument = new Vocals();

        foreach (var diff in EnumCache<Difficulty>.Values)
        {
            var track = instument.CreateTrack(diff);
            track.Chords.AddRange(lyrics);
        }

        return instument;
    }
    #endregion

    #region Drums
    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static Drums? ReadDrums(string path, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstruments(path, new(InstrumentIdentity.Drums), config, formatting).Drums;

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<Drums?> ReadDrumsAsync(string path, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => (await ReadInstrumentsAsync(path, new(InstrumentIdentity.Drums), config, formatting)).Drums;
    #endregion

    #region GHL
    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static GHLInstrument? ReadInstrument(string path, GHLInstrumentIdentity instrument, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstruments(path, new(instrument, difficulties), config, formatting).Get(instrument);

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<GHLInstrument?> ReadInstrumentAsync(string path, GHLInstrumentIdentity instrument, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => (await ReadInstrumentsAsync(path, new(instrument, difficulties), config, formatting, cancellationToken)).Get(instrument);
    #endregion

    #region Standard
    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static StandardInstrument? ReadInstrument(string path, StandardInstrumentIdentity instrument, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstruments(path, new(instrument, difficulties), config, formatting).Get(instrument);

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<StandardInstrument?> ReadInstrumentAsync(string path, StandardInstrumentIdentity instrument, DifficultySet difficulties = DifficultySet.All, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => (await ReadInstrumentsAsync(path, new(instrument, difficulties), config, formatting, cancellationToken)).Get(instrument);
    #endregion
    #endregion

    #region Tracks
    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static Track? ReadTrack(string path, InstrumentIdentity instrument, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstrument(path, instrument, difficulty.ToSet(), config, formatting)?.GetTrack(difficulty);

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<Track?> ReadTrackAsync(string path, InstrumentIdentity instrument, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => (await ReadInstrumentAsync(path, instrument, difficulty.ToSet(), config, formatting, cancellationToken))?.GetTrack(difficulty);

    #region Drums
    [Obsolete($"Use {nameof(ReadDrums)} with a {nameof(DifficultySet)}.")]
    public static Track<DrumsChord> ReadDrumsTrack(string path, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadDrums(path, difficulty.ToSet(), config, formatting)?.GetTrack(difficulty) ?? new();

    [Obsolete($"Use {nameof(ReadDrumsAsync)} with a {nameof(DifficultySet)}.")]
    public static async Task<Track<DrumsChord>> ReadDrumsTrackAsync(string path, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => (await ReadDrumsAsync(path, difficulty.ToSet(), config, formatting, cancellationToken))?.GetTrack(difficulty) ?? new();
    #endregion

    #region GHL
    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static Track<GHLChord>? ReadTrack(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstrument(path, instrument, difficulty.ToSet(), config, formatting)?.GetTrack(difficulty);

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<Track<GHLChord>?> ReadTrackAsync(string path, GHLInstrumentIdentity instrument, Difficulty difficulty, ChartReadingConfiguration? config, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
        => (await ReadInstrumentAsync(path, instrument, difficulty.ToSet(), config, formatting, cancellationToken))?.GetTrack(difficulty);
    #endregion

    #region Standard
    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static Track<StandardChord>? ReadTrack(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
        => ReadInstrument(path, instrument, difficulty.ToSet(), config, formatting)?.GetTrack(difficulty);

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task<Track<StandardChord>?> ReadTrackAsync(string path, StandardInstrumentIdentity instrument, Difficulty difficulty, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
         => (await ReadInstrumentAsync(path, instrument, difficulty.ToSet(), config, formatting, cancellationToken))?.GetTrack(difficulty);
    #endregion
    #endregion

    #region Metadata

    public static Metadata ReadMetadata(string path)
        => ReadMetadata(new ReadingDataSource(path));

    public static Metadata ReadMetadata(Stream stream)
        => ReadMetadata(new ReadingDataSource(stream));

    /// <summary>
    /// Reads metadata from a chart file.
    /// </summary>
    public static Metadata ReadMetadata(ReadingDataSource source)
    {
        var session = new ChartReadingSession(new() { Metadata = true }, DefaultReadConfig, null);
        var reader = new ChartFileReader(source, session);

        reader.Read();
        return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
    }

    public static Task<Metadata> ReadMetadata(string path, CancellationToken cancellationToken = default)
       => ReadMetadata(new ReadingDataSource(path), cancellationToken);

    public static Task<Metadata> ReadMetadata(Stream stream, CancellationToken cancellationToken = default)
        => ReadMetadata(new ReadingDataSource(stream), cancellationToken);

    public static async Task<Metadata> ReadMetadata(ReadingDataSource source, CancellationToken cancellationToken = default)
    {
        var session = new ChartReadingSession(new() { Metadata = true }, DefaultReadConfig, null);
        var reader = new ChartFileReader(source, session);

        await reader.ReadAsync(cancellationToken);
        return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
    }
    #endregion

    #region Global events
    public static List<GlobalEvent> ReadGlobalEvents(string path)
        => ReadGlobalEvents(new ReadingDataSource(path));

    public static List<GlobalEvent> ReadGlobalEvents(Stream stream)
    => ReadGlobalEvents(new ReadingDataSource(stream));

    public static List<GlobalEvent> ReadGlobalEvents(ReadingDataSource source)
    {
        var session = new ChartReadingSession(new() { GlobalEvents = true }, DefaultReadConfig, null);
        var reader = new ChartFileReader(source, session);

        reader.Read();
        return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : [];
    }

    public static Task<List<GlobalEvent>> ReadGlobalEventsAsync(string path, CancellationToken cancellationToken = default)
        => ReadGlobalEventsAsync(new ReadingDataSource(path), cancellationToken);

    public static Task<List<GlobalEvent>> ReadGlobalEventsAsync(Stream stream, CancellationToken cancellationToken = default)
    => ReadGlobalEventsAsync(new ReadingDataSource(stream), cancellationToken);

    public static async Task<List<GlobalEvent>> ReadGlobalEventsAsync(ReadingDataSource source, CancellationToken cancellationToken = default)
    {
        var session = new ChartReadingSession(new() { GlobalEvents = true }, DefaultReadConfig, null);
        var reader = new ChartFileReader(source, session);

        await reader.ReadAsync(cancellationToken);
        return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : [];
    }

    public static IEnumerable<Phrase> ReadLyrics(string path) => ReadLyrics(new ReadingDataSource(path));

    public static IEnumerable<Phrase> ReadLyrics(Stream stream) => ReadLyrics(new ReadingDataSource(stream));

    /// <summary>
    /// Reads lyrics from a chart file.
    /// </summary>
    /// <returns>Enumerable of <see cref="Phrase"/> containing the lyrics from the file</returns>
    /// <param name="path">Path of the file to read</param>
    /// <inheritdoc cref="ReadGlobalEvents(string)" path="/exception"/>
    public static IEnumerable<Phrase> ReadLyrics(ReadingDataSource source) => ReadGlobalEvents(source).GetLyrics();

    public static Task<IEnumerable<Phrase>> ReadLyricsAsync(string path, CancellationToken cancellationToken = default)
        => ReadLyricsAsync(new ReadingDataSource(path), cancellationToken);

    public static Task<IEnumerable<Phrase>> ReadLyricsAsync(Stream stream, CancellationToken cancellationToken = default)
    => ReadLyricsAsync(new ReadingDataSource(stream), cancellationToken);

    /// <summary>
    /// Reads lyrics from a chart file asynchronously using multitasking.
    /// </summary>
    /// <param name="cancellationToken">Token to request cancellation</param>
    public static async Task<IEnumerable<Phrase>> ReadLyricsAsync(ReadingDataSource source, CancellationToken cancellationToken = default)
        => (await ReadGlobalEventsAsync(source, cancellationToken)).GetLyrics();
    #endregion

    #region Sync track
    /// <inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)"/>
    /// <param name="path"><inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)" path="/param[@name='path']"/></param>
    /// <param name="config"><inheritdoc cref="SyncTrack.FromFile(string, ReadingConfiguration?)" path="/param[@name='config']"/></param>
    public static SyncTrack ReadSyncTrack(string path, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
    {
        var session = new ChartReadingSession(new() { SyncTrack = true }, config, null);
        var reader = new ChartFileReader(new(path), session);

        reader.Read();
        return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
    }

    /// <inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)"/>
    /// <param name="path"><inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)" path="/param[­@name='path']"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)" path="/param[­@name='cancellationToken']"/></param>
    /// <param name="config"><inheritdoc cref="SyncTrack.FromFileAsync(string, ReadingConfiguration?, CancellationToken)" path="/param[­@name='config']"/></param>
    public static async Task<SyncTrack> ReadSyncTrackAsync(string path, ChartReadingConfiguration? config = default, CancellationToken cancellationToken = default)
    {
        var session = new ChartReadingSession(new() { SyncTrack = true }, config, null);
        var reader = new ChartFileReader(new(path), session);

        await reader.ReadAsync(cancellationToken);
        return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
    }
    #endregion
    #endregion

    #region Writing
    private static void FillInstrumentsWriterData(InstrumentSet set, InstrumentComponentList components, ChartWritingSession session,
        List<Serializer<string>> serializers, List<string> removedHeaders)
    {
        foreach (var identity in
            EnumCache<StandardInstrumentIdentity>.Values.Cast<InstrumentIdentity>()
            .Concat(EnumCache<GHLInstrumentIdentity>.Values.Cast<InstrumentIdentity>())
            .Append(InstrumentIdentity.Drums))
        {
            var tracks = components.Map(identity);

            // Only act on tracks specified in the component list
            foreach (var diff in EnumCache<Difficulty>.Values.Where(d => tracks.HasFlag(d.ToSet())))
            {
                var track = set.Get(identity)?.GetTrack(diff);

                if (track?.IsEmpty is not null or false)
                    serializers.Add(new TrackSerializer(track, session));
                else // No track data for the instrument and difficulty
                    removedHeaders.Add(ChartFormatting.Header(identity, diff));
            }
        }
    }

    private static ChartFileWriter GetSongWriter(WritingDataSource source, Song song, ComponentList components, ChartWritingSession session)
    {
        var removedHeaders = new List<string>();
        var serializers = new List<Serializer<string>>();

        if (components.Metadata)
            serializers.Add(new MetadataSerializer(song.Metadata));

        if (components.SyncTrack)
        {
            if (!song.SyncTrack.IsEmpty)
                serializers.Add(new SyncTrackSerializer(song.SyncTrack, session));
            else
                removedHeaders.Add(ChartFormatting.SyncTrackHeader);
        }

        if (components.GlobalEvents)
        {
            if (song.GlobalEvents.Count > 0)
                serializers.Add(new GlobalEventSerializer(song.GlobalEvents, session));
            else
                removedHeaders.Add(ChartFormatting.GlobalEventHeader);
        }

        FillInstrumentsWriterData(song.Instruments, components.Instruments, session, serializers, removedHeaders);

        if (song.UnknownChartSections is not null)
            serializers.AddRange(song.UnknownChartSections.Select(s => new UnknownSectionSerializer(s.Header, s, session)));

        return new(source, removedHeaders, [.. serializers]);
    }

    public static void WriteSong(string path, Song song, ChartWritingConfiguration? config = default)
        => WriteSong(new WritingDataSource(path), song, config);
    public static void WriteSong(Stream stream, Song song, ChartWritingConfiguration? config = default)
    => WriteSong(new WritingDataSource(stream), song, config);

    /// <summary>
    /// Writes a song to a chart file.
    /// </summary>
    /// <param name="path">Path of the file to write</param>
    /// <param name="song">Song to write</param>
    public static void WriteSong(WritingDataSource source, Song song, ChartWritingConfiguration? config = default)
    {
        var writer = GetSongWriter(source, song, ComponentList.Full(), new(config, song.Metadata.Formatting));
        writer.Write();
    }

    public static Task WriteSongAsync(string path, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
        => WriteSongAsync(new WritingDataSource(path), song, config, cancellationToken);

    public static Task WriteSongAsync(Stream stream, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    => WriteSongAsync(new WritingDataSource(stream), song, config, cancellationToken);

    public static async Task WriteSongAsync(WritingDataSource source, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    {
        var writer = GetSongWriter(source, song, ComponentList.Full(), new(config, song.Metadata.Formatting));
        await writer.WriteAsync(cancellationToken);
    }

    public static void ReplaceComponents(string path, Song song, ComponentList components, ChartWritingConfiguration? config = default)
        => ReplaceComponents(new WritingDataSource(path), song, components, config);

    public static void ReplaceComponents(Stream stream, Song song, ComponentList components, ChartWritingConfiguration? config = default)
        => ReplaceComponents(new WritingDataSource(stream), song, components, config);

    public static void ReplaceComponents(string path, string existing, Song song, ComponentList components, ChartWritingConfiguration? config = default)
        => ReplaceComponents(new WritingDataSource(path, new ReadingDataSource(existing)), song, components, config);

    public static void ReplaceComponents(Stream stream, Stream existing, Song song, ComponentList components, ChartWritingConfiguration? config = default)
        => ReplaceComponents(new WritingDataSource(stream, new ReadingDataSource(existing)), song, components, config);

    public static void ReplaceComponents(WritingDataSource source, Song song, ComponentList components, ChartWritingConfiguration? config = default)
    {
        var writer = GetSongWriter(source, song, components, new(config, song.Metadata.Formatting));
        writer.Write();
    }

    public static Task ReplaceComponentsAsync(string path, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
        => ReplaceComponentsAsync(new WritingDataSource(path), song, components, config, cancellationToken);

    public static Task ReplaceComponentsAsync(Stream stream, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    => ReplaceComponentsAsync(new WritingDataSource(stream), song, components, config, cancellationToken);

    public static Task ReplaceComponentsAsync(string path, string existing, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    => ReplaceComponentsAsync(new WritingDataSource(path, new(existing)), song, components, config, cancellationToken);

    public static Task ReplaceComponentsAsync(Stream stream, Stream existing, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    => ReplaceComponentsAsync(new WritingDataSource(stream, new(existing)), song, components, config, cancellationToken);

    public static async Task ReplaceComponentsAsync(WritingDataSource source, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    {
        var writer = GetSongWriter(source, song, components, new(config, song.Metadata.Formatting));
        await writer.WriteAsync(cancellationToken);
    }

    private static ChartFileWriter GetInstrumentsWriter(WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingSession session)
    {
        var serializers = new List<Serializer<string>>();
        var removedHeaders = new List<string>();

        FillInstrumentsWriterData(set, components, session, serializers, removedHeaders);

        return new(source, removedHeaders, [.. serializers]);
    }

    public static void ReplaceInstruments(string path, InstrumentSet set, InstrumentComponentList components, ChartWritingConfiguration? config = default, FormattingRules? formatting = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetInstrumentsWriter(source, set, components, new(config, formatting));
        writer.Write();
    }

    public static async Task ReplaceInstrumentsAsync(string path, InstrumentSet set, InstrumentComponentList components, ChartWritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetInstrumentsWriter(source, set, components, new(config, formatting));
        await writer.WriteAsync(cancellationToken);
    }

    [Obsolete($"Use {nameof(ReadInstruments)} with a component list.")]
    public static void ReplaceInstrument(string path, Instrument instrument, DifficultySet diffs = DifficultySet.All, ChartWritingConfiguration? config = default, FormattingRules? formatting = default)
    {
        var set = new InstrumentSet();
        set.Set(instrument);

        ReplaceInstruments(path, set, new(instrument.InstrumentIdentity, diffs), config, formatting);
    }

    [Obsolete($"Use {nameof(ReadInstrumentsAsync)} with a component list.")]
    public static async Task ReplaceInstrumentAsync(string path, Instrument instrument, DifficultySet diffs = DifficultySet.All, ChartWritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        var set = new InstrumentSet();
        set.Set(instrument);

        await ReplaceInstrumentsAsync(path, set, new(instrument.InstrumentIdentity, diffs), config, formatting, cancellationToken);
    }

    private static ChartFileWriter GetTrackWriter(WritingDataSource source, Track track, ChartWritingSession session)
    {
        if (track.ParentInstrument is null)
            throw new ArgumentNullException(nameof(track), "Cannot write track because it does not belong to an instrument.");
        if (!Enum.IsDefined(track.ParentInstrument.InstrumentIdentity))
            throw new ArgumentException("Cannot write track because the instrument it belongs to is unknown.", nameof(track));

        return new(source, null, new TrackSerializer(track, session));
    }

    [Obsolete($"Use {nameof(ReplaceInstrument)} with a {nameof(DifficultySet)}.")]
    public static void ReplaceTrack(string path, Track track, ChartWritingConfiguration? config = default, FormattingRules? formatting = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetTrackWriter(source, track, new(config, formatting));
        writer.Write();
    }

    [Obsolete($"Use {nameof(ReplaceInstrumentAsync)} with a {nameof(DifficultySet)}.")]
    public static async Task ReplaceTrackAsync(string path, Track track, ChartWritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetTrackWriter(source, track, new(config, formatting));
        await writer.WriteAsync(cancellationToken);
    }

    private static ChartFileWriter GetMetadataWriter(WritingDataSource source, Metadata metadata) => new(source, null, new MetadataSerializer(metadata));

    /// <summary>
    /// Replaces the metadata in a file.
    /// </summary>
    /// <param name="path">Path of the file to read</param>
    /// <param name="metadata">Metadata to write</param>
    public static void ReplaceMetadata(string path, Metadata metadata)
    {
        using var source = new WritingDataSource(path);

        var writer = GetMetadataWriter(source, metadata);
        writer.Write();
    }

    private static ChartFileWriter GetGlobalEventWriter(WritingDataSource source, IEnumerable<GlobalEvent> events, ChartWritingSession session) => new(source, null, new GlobalEventSerializer(events, session));

    /// <summary>
    /// Replaces the global events in a file.
    /// </summary>
    /// <param name="path">Path of the file to write</param>
    /// <param name="events">Events to use as a replacement</param>
    public static void ReplaceGlobalEvents(string path, IEnumerable<GlobalEvent> events)
    {
        using var source = new WritingDataSource(path);

        var writer = GetGlobalEventWriter(source, events, new(DefaultWriteConfig, null));
        writer.Write();
    }

    public static async Task ReplaceGlobalEventsAsync(string path, IEnumerable<GlobalEvent> events, CancellationToken cancellationToken = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetGlobalEventWriter(source, events, new(DefaultWriteConfig, null));
        await writer.WriteAsync(cancellationToken);
    }

    private static ChartFileWriter GetSyncTrackWriter(WritingDataSource source, SyncTrack syncTrack, ChartWritingSession session) => new(source, null, new SyncTrackSerializer(syncTrack, session));

    /// <summary>
    /// Replaces the sync track in a file.
    /// </summary>
    /// <param name="path">Path of the file to write</param>
    /// <param name="syncTrack">Sync track to write</param>
    /// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
    public static void ReplaceSyncTrack(string path, SyncTrack syncTrack, ChartWritingConfiguration? config = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetSyncTrackWriter(source, syncTrack, new(config, null));
        writer.Write();
    }

    public static async Task ReplaceSyncTrackAsync(string path, SyncTrack syncTrack, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
    {
        using var source = new WritingDataSource(path);

        var writer = GetSyncTrackWriter(source, syncTrack, new(config, null));
        await writer.WriteAsync(cancellationToken);
    }
    #endregion

    /// <summary>
    /// Gets all the combinations of instruments and difficulties.
    /// </summary>
    /// <param name="instruments">Enum containing the instruments</param>
    private static IEnumerable<(Difficulty difficulty, TInstEnum instrument)> GetTrackCombinations<TInstEnum>(IEnumerable<TInstEnum> instruments) => from difficulty in EnumCache<Difficulty>.Values from instrument in instruments select (difficulty, instrument);
}
