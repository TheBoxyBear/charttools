using ChartTools.Events;
using ChartTools.Extensions.Enums;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Parsing;
using ChartTools.IO.Chart.Serializing;
using ChartTools.IO.Components;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.IO.Serializing;
using ChartTools.IO.Sources;
using ChartTools.Lyrics;
using ChartTools.Meta;

namespace ChartTools.IO.Chart;

/// <summary>
/// Provides methods for reading and writing chart files
/// </summary>
public static class ChartFile
{
	/// <summary>
	/// Default configuration to use for reading when the provided configuration is <see langword="default"/>
	/// </summary>
	public static ChartReadingConfiguration DefaultReadConfig { get; set; } = new(false)
	{
		DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
		OverlappingStarPowerPolicy = OverlappingSpecialPhrasePolicy.ThrowException,
		SnappedNotesPolicy         = SnappedNotesPolicy.ThrowException,
		SoloNoStarPowerPolicy      = SoloNoStarPowerPolicy.Convert,
		TempolessAnchorPolicy      = TempolessAnchorPolicy.ThrowException,
		UnknownSectionPolicy       = UnknownSectionPolicy.ThrowException
	};

	/// <summary>
	/// Default configuration to use for writing when the provided configuration is <see langword="default"/>
	/// </summary>
	public static ChartWritingConfiguration DefaultWriteConfig { get; set; } = new(false)
	{
		DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.ThrowException,
		OverlappingStarPowerPolicy = OverlappingSpecialPhrasePolicy.ThrowException,
		SoloNoStarPowerPolicy      = SoloNoStarPowerPolicy.Convert,
		SnappedNotesPolicy         = SnappedNotesPolicy.ThrowException,
		UnsupportedModifierPolicy  = UnsupportedModifierPolicy.ThrowException
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

		foreach (ChartParser parser in reader.Parsers)
			parser.ApplyToSong(song);

		// Vocals are read and parsed as global events
		if (reader.Session.Components.Vocals)
		{
			// Parson of global events is forced - not null
			song.GlobalEvents!.GetLyrics(out IList<PhraseMarker>? phrases, out IList<VocalsNote>? notes);
			song.Vocals = new() { Standard = new(phrases, notes) };
		}

		// Requesting the vocals components forces parsing of global events. Discard if not requested.
		if (!reader.Session.Components.GlobalEvents)
			song.GlobalEvents?.Clear();

		return song;
	}

	/// <summary>
	/// Reads a <see cref="Song"/> from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="config">Optional read configuration</param>
	public static Song ReadSong(
		ReadingDataSource source, ChartReadingConfiguration? config = default)
	{
		ChartReadingSession session  = new(ComponentList.Full(), config);
		using ChartFileReader reader = new(source, session);

		reader.Read();
		return CreateSongFromReader(reader);
	}

	/// <summary>
	/// Reads a <see cref="Song"/> from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="config">Optional read configuration</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <remarks>Uses multi-threading to parse song components.</remarks>
	public static async Task<Song> ReadSongAsync(
		ReadingDataSource source, ChartReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		ChartReadingSession session  = new(ComponentList.Full(), config);
		using ChartFileReader reader = new(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return CreateSongFromReader(reader);
	}

	/// <summary>
	/// Reads a a set of <see cref="Song"/> components from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="components">Set of components to read</param>
	/// <param name="config">Optional read configuration</param>
	public static Song ReadComponents(
		ReadingDataSource source, ComponentList components, ChartReadingConfiguration? config = default)
	{
		ChartReadingSession session  = new(components, config);
		using ChartFileReader reader = new(source, session);

		reader.Read();
		return CreateSongFromReader(reader);
	}

	/// <summary>
	/// Reads a a set of <see cref="Song"/> components from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="components">Set of components to read</param>
	/// <param name="config">Optional read configuration</param>
	/// <param name="cancellationToken">Token used for cancellation</param>v
	/// <remarks>Uses multi-threading to parse song components.</remarks>
	public static async Task<Song> ReadComponentsAsync(
		ReadingDataSource source, ComponentList components, ChartReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		ChartReadingSession session  = new(components, config);
		using ChartFileReader reader = new(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return CreateSongFromReader(reader);
	}
	#endregion

	#region Instruments
	private static InstrumentSet CreateInstrumentSetFromReader(ChartFileReader reader)
	{
		InstrumentSet instruments = new();

		foreach (ChartParser parser in reader.Parsers)
			switch (parser)
			{
				case DrumsTrackParser drumsParser:
					instruments.Drums ??= new();
					drumsParser.ApplyToInstrument(instruments.Drums);
					break;
				case StandardTrackParser standardParser:
					StandardInstrument? standardInst = instruments.Get(standardParser.Instrument);

					if (standardInst is null)
					{
						standardInst = new StandardInstrument(standardParser.Instrument);
						instruments.Set(standardInst);
					}

					standardParser.ApplyToInstrument(standardInst);
					break;
				case GHLTrackParser ghlParser:
					GHLInstrument? ghlInst = instruments.Get(ghlParser.Instrument);

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

	/// <summary>
	/// Reads a set of <see cref="Instrument"/> from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="components">Instruments to read</param>
	/// <param name="config">Optional read config</param>
	public static InstrumentSet ReadInstruments(
		ReadingDataSource source, InstrumentComponentList components, ChartReadingConfiguration? config = default)
	{
		ChartReadingSession session  = new(new() { Instruments = components }, config);
		using ChartFileReader reader = new(source, session);

		reader.Read();
		return CreateInstrumentSetFromReader(reader);
	}

	/// <summary>
	/// Reads a set of <see cref="Instrument"/> from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="components">Instruments to read</param>
	/// <param name="config">Optional read config</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <remarks>Uses multi-threading to parse tracks.</remarks>
	public static async Task<InstrumentSet> ReadInstrumentsAsync(
		ReadingDataSource source, InstrumentComponentList components, ChartReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		ChartReadingSession session  = new(new() { Instruments = components }, config);
		using ChartFileReader reader = new(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return CreateInstrumentSetFromReader(reader);
	}
	#endregion

	#region Metadata
	/// <summary>
	/// Reads the <see cref="Metadata"/> from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="existing"><see cref="Metadata"/> from another target to combine with</param>
	/// <returns><see cref="Metadata"/> object provided as the <paramref name="existing"/> parameter, or a new instance if passed <see langword="null"/>.</returns>
	public static Metadata ReadMetadata(ReadingDataSource source, Metadata? existing = null)
	{
		ChartReadingSession session  = new(new() { Metadata = true }, DefaultReadConfig);
		using ChartFileReader reader = new(source, session);

		reader.ExistingMetadata = existing;
		reader.Read();
		return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
	}

	/// <summary>
	/// Reads the <see cref="Metadata"/> from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="existing"><see cref="Metadata"/> from another target to combine with</param>
	/// <param name="cancellationToken">Token to request cancellation</param>
	/// <returns><see cref="Metadata"/> object provided as the <paramref name="existing"/> parameter, or a new instance if passed <see langword="null"/>.</returns>
	public static async Task<Metadata> ReadMetadataAsync(
		ReadingDataSource source, Metadata? existing = null, CancellationToken cancellationToken = default)
	{
		ChartReadingSession session  = new(new() { Metadata = true }, DefaultReadConfig);
		using ChartFileReader reader = new(source, session);

		reader.ExistingMetadata = existing;
		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
	}
	#endregion

	#region Global events
	/// <summary>
	/// Reads the set of <see cref="GlobalEvent"/> from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	public static List<GlobalEvent> ReadGlobalEvents(ReadingDataSource source)
	{
		ChartReadingSession session  = new(new() { GlobalEvents = true }, DefaultReadConfig);
		using ChartFileReader reader = new(source, session);

		reader.Read();
		return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result : [];
	}

	/// <summary>
	/// Reads the set of <see cref="GlobalEvent"/> from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task<List<GlobalEvent>> ReadGlobalEventsAsync(
		ReadingDataSource source, CancellationToken cancellationToken = default)
	{
		ChartReadingSession session  = new(new() { GlobalEvents = true }, DefaultReadConfig);
		using ChartFileReader reader = new(source, session);

		await reader.ReadAsync(cancellationToken);
		return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result : [];
	}
	#endregion

	#region Vocals
	/// <summary>
	/// Reads the <see cref="StandardVocalsTrack"/> from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	public static StandardVocalsTrack ReadVocals(ReadingDataSource source)
	{
		ReadGlobalEvents(source).GetLyrics(out IList<PhraseMarker>? phrases, out IList<VocalsNote>? notes);
		return new(phrases, notes);
	}

	/// <summary>
	/// Reads the <see cref="StandardVocalsTrack"/> from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task<StandardVocalsTrack> ReadVocalsAsync(
		ReadingDataSource source, CancellationToken cancellationToken = default)
	{
		(await ReadGlobalEventsAsync(source, cancellationToken)).GetLyrics(out IList<PhraseMarker>? phrases, out IList<VocalsNote>? notes);
		return new(phrases, notes);
	}
	#endregion

	#region Sync track
	/// <summary>
	/// Reads the <see cref="SyncTrack"/> from a chart target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="config">Optional read config</param>
	public static SyncTrack ReadSyncTrack(ReadingDataSource source, ChartReadingConfiguration? config = default)
	{
		ChartReadingSession session  = new(new() { SyncTrack = true }, config);
		using ChartFileReader reader = new(source, session);

		reader.Read();
		return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result : new();
	}

	/// <summary>
	/// Reads the <see cref="SyncTrack"/> from a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="config">Optional read config</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task<SyncTrack> ReadSyncTrackAsync(
		ReadingDataSource source, ChartReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		ChartReadingSession session  = new(new() { SyncTrack = true }, config);
		using ChartFileReader reader = new(source, session);

		await reader.ReadAsync(cancellationToken);
		return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result : new();
	}
	#endregion
	#endregion

	#region Writing
	private static void FillInstrumentsWriterData(
		InstrumentSet set, InstrumentComponentList components, ChartWritingSession session, List<Serializer<string>> serializers, List<string> removedHeaders)
	{
		foreach (InstrumentIdentity identity in
			EnumCache<StandardInstrumentIdentity>.Values.Cast<InstrumentIdentity>()
			.Concat(EnumCache<GHLInstrumentIdentity>.Values.Cast<InstrumentIdentity>())
			.Append(InstrumentIdentity.Drums))
		{
			DifficultySet tracks = components.Map(identity);

			// Only act on tracks specified in the component list
			foreach (Difficulty diff in EnumCache<Difficulty>.Values.Where(d => tracks.HasFlag(d.ToSet())))
			{
				Track? track = set.Get(identity)?.GetTrack(diff);

				if (track?.IsEmpty is not null or false)
					serializers.Add(new TrackSerializer(track, session));
				else // No track data for the instrument and difficulty
					removedHeaders.Add(ChartFormatting.Header(identity, diff));
			}
		}
	}

	#region Song
	private static ChartFileWriter GetSongWriter(
		WritingDataSource source, Song song, ComponentList components, ChartWritingSession session)
	{
		List<string> removedHeaders = [];
		List<Serializer<string>> serializers = [];

		if (components.Metadata)
		{
			if (song.Metadata is not null)
				serializers.Add(new MetadataSerializer(song.Metadata));
			else
				removedHeaders.Add(ChartFormatting.MetadataHeader);
		}

		if (components.SyncTrack)
		{
			if (song.SyncTrack is not null)
				serializers.Add(new SyncTrackSerializer(song.SyncTrack, session));
			else
				removedHeaders.Add(ChartFormatting.SyncTrackHeader);
		}

		if (components.GlobalEvents || components.Vocals)
		{
			if (components.Vocals)
			{
				StandardVocalsTrack vocals = (song.Vocals ??= new()).Standard;

				song.GlobalEvents = [.. song.GlobalEvents is null
					? vocals.ToGlobalEvents()
					: song.GlobalEvents.SetLyrics(vocals)];
			}

			if (song.GlobalEvents?.Count > 0)
				serializers.Add(new GlobalEventSerializer(song.GlobalEvents, session));
			else
				removedHeaders.Add(ChartFormatting.GlobalEventHeader);
		}

		FillInstrumentsWriterData(song.Instruments, components.Instruments, session, serializers, removedHeaders);

		if (song.UnknownChartSections is not null)
			serializers.AddRange(song.UnknownChartSections.Select(s => new UnknownSectionSerializer(s.Header, s, session)));

		return new(source, removedHeaders, [.. serializers]);
	}

	/// <summary>
	/// Writes a <see cref="Song"/> to a chart target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="song">Song to write</param>
	/// <param name="config">Optional write configuration</param>
	public static void WriteSong(WritingDataSource source, Song song, ChartWritingConfiguration? config = default)
	{
		using ChartFileWriter writer = GetSongWriter(source, song, ComponentList.Full(), new(config, song.Metadata?.Formatting));
		writer.Write();
	}

	/// <summary>
	/// Writes a <see cref="Song"/> to a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="song">Song to write</param>
	/// <param name="config">Optional write configuration</param>
	/// <param name="cancellationToken">Token to request cancellation</param>
	/// <remarks>Uses multi-threading to serialize song components.</remarks>
	public static async Task WriteSongAsync(
		WritingDataSource source, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		using ChartFileWriter writer = GetSongWriter(source, song, ComponentList.Full(), new(config, song.Metadata?.Formatting));
		await writer.WriteAsync(cancellationToken);
	}
	#endregion

	#region Components
	/// <summary>
	/// Replaces a set of <see cref="Song"/> components in a chart target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="song">Song data to write</param>
	/// <param name="components">Set of components to replace</param>
	/// <param name="config">Optional write config</param>
	public static void ReplaceComponents(
		WritingDataSource source, Song song, ComponentList components, ChartWritingConfiguration? config = default)
	{
		using ChartFileWriter writer = GetSongWriter(source, song, components, new(config, song.Metadata?.Formatting));
		writer.Write();
	}

	/// <summary>
	/// Replaces a set of <see cref="Song"/> components in a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="song">Song data to write</param>
	/// <param name="components">Set of components to replace</param>
	/// <param name="config">Optional write config</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <remarks>Uses multi-threading to serialize song components.</remarks>
	public static async Task ReplaceComponentsAsync(
		WritingDataSource source, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		using ChartFileWriter writer = GetSongWriter(source, song, components, new(config, song.Metadata?.Formatting));
		await writer.WriteAsync(cancellationToken);
	}
	#endregion

	#region Instruments
	private static ChartFileWriter GetInstrumentsWriter(
		WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingSession session)
	{
		List<Serializer<string>> serializers = [];
		List<string> removedHeaders = [];

		FillInstrumentsWriterData(set, components, session, serializers, removedHeaders);

		return new(source, removedHeaders, [.. serializers]);
	}

	/// <summary>
	/// Replaces a set of instruments in a chart target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="set">Instrument data to write</param>
	/// <param name="components">Set of instruments and tracks to replace</param>
	/// <param name="config">Optional write config</param>
	/// <param name="formatting">Formatting to apply</param>
	public static void ReplaceInstruments(
		WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingConfiguration? config = default, FormattingRules? formatting = default)
	{
		using ChartFileWriter writer = GetInstrumentsWriter(source, set, components, new(config, formatting));
		writer.Write();
	}

	/// <summary>
	/// Replaces a set of instruments in a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="set">Instrument data to write</param>
	/// <param name="components">Set of instruments and tracks to replace</param>
	/// <param name="config">Optional write config</param>
	/// <param name="formatting">Formatting to apply</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task ReplaceInstrumentsAsync(
		WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
	{
		using ChartFileWriter writer = GetInstrumentsWriter(source, set, components, new(config, formatting));
		await writer.WriteAsync(cancellationToken);
	}
	#endregion

	#region Metadata
	private static ChartFileWriter GetMetadataWriter(WritingDataSource source, Metadata metadata)
		=> new(source, null, new MetadataSerializer(metadata));

	/// <summary>
	/// Replaces the <see cref="Metadata"/> in a chart target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="metadata">Metadata to write</param>
	public static void ReplaceMetadata(WritingDataSource source, Metadata metadata)
	{
		using ChartFileWriter writer = GetMetadataWriter(source, metadata);
		writer.Write();
	}

	/// <summary>
	/// Replaces the <see cref="Metadata"/> in a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="metadata">Metadata to write</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task ReplaceMetadataAsync(
		WritingDataSource source, Metadata metadata, CancellationToken cancellationToken = default)
	{
		using ChartFileWriter writer = GetMetadataWriter(source, metadata);
		await writer.WriteAsync(cancellationToken);
	}
	#endregion

	#region Global events
	private static ChartFileWriter GetGlobalEventWriter(
		WritingDataSource source, IEnumerable<GlobalEvent> events, ChartWritingSession session)
		=> new(source, null, new GlobalEventSerializer(events, session));

	/// <summary>
	/// Replaces the set of <see cref="GlobalEvent"/> in a chart target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="events">Events to use as a replacement</param>
	public static void ReplaceGlobalEvents(WritingDataSource source, IEnumerable<GlobalEvent> events)
	{
		using ChartFileWriter writer = GetGlobalEventWriter(source, events, new(DefaultWriteConfig, null));
		writer.Write();
	}

	/// <summary>
	/// Replaces the set of <see cref="GlobalEvent"/> in a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="events">Events to use as a replacement</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task ReplaceGlobalEventsAsync(
		WritingDataSource source, IEnumerable<GlobalEvent> events, CancellationToken cancellationToken = default)
	{
		using ChartFileWriter writer = GetGlobalEventWriter(source, events, new(DefaultWriteConfig, null));
		await writer.WriteAsync(cancellationToken);
	}
	#endregion

	private static ChartFileWriter GetSyncTrackWriter(WritingDataSource source, SyncTrack syncTrack, ChartWritingSession session)
		=> new(source, null, new SyncTrackSerializer(syncTrack, session));

	/// <summary>
	/// Replaces the sync track in a chart target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="syncTrack">Sync track to write</param>
	/// <param name="config">Optional write configuration</param>
	public static void ReplaceSyncTrack(
		WritingDataSource source, SyncTrack syncTrack, ChartWritingConfiguration? config = default)
	{
		using ChartFileWriter writer = GetSyncTrackWriter(source, syncTrack, new(config, null));
		writer.Write();
	}

	/// <summary>
	/// Replaces the sync track in a chart target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="syncTrack">Sync track to write</param>
	/// <param name="config">Optional write configuration</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task ReplaceSyncTrackAsync(
		WritingDataSource source, SyncTrack syncTrack, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		using ChartFileWriter writer = GetSyncTrackWriter(source, syncTrack, new(config, null));
		await writer.WriteAsync(cancellationToken);
	}
	#endregion
}
