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
using ChartTools.Lyrics.Tracks;

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

		// Vocals are read and parsed as global events
		if (reader.Session.Components.Vocals)
		{
			// Parson of global events is forced - not null
			song.GlobalEvents!.GetLyrics(out var phrases, out var notes);
			song.Vocals = new() { Standard = new(phrases, notes) };
		}

		// Requesting the vocals components forces parsing of global events. Discard if not requested.
		if (!reader.Session.Components.GlobalEvents)
			song.GlobalEvents?.Clear();

		return song;
	}

	public static Song ReadSong(ReadingDataSource source, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
	{
		var session = new ChartReadingSession(ComponentList.Full(), config, formatting);
		using var reader = new ChartFileReader(source, session);

		reader.Read();
		return CreateSongFromReader(reader);
	}

	public static async Task<Song> ReadSongAsync(ReadingDataSource source, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
	{
		var session = new ChartReadingSession(ComponentList.Full(), config, formatting);
		using var reader = new ChartFileReader(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return CreateSongFromReader(reader);
	}

	public static Song ReadComponents(
		ReadingDataSource source, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
	{
		var session = new ChartReadingSession(components, config, formatting);
		using var reader = new ChartFileReader(source, session);

		reader.Read();
		return CreateSongFromReader(reader);
	}

	public static async Task<Song> ReadComponentsAsync(
		ReadingDataSource source, ComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
	{
		var session = new ChartReadingSession(components, config, formatting);
		using var reader = new ChartFileReader(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
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

	public static InstrumentSet ReadInstruments(ReadingDataSource source, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default)
	{
		var session = new ChartReadingSession(new() { Instruments = components }, config, formatting);
		using var reader = new ChartFileReader(source, session);

		reader.Read();
		return CreateInstrumentSetFromReader(reader);
	}

	public static async Task<InstrumentSet> ReadInstrumentsAsync(ReadingDataSource source, InstrumentComponentList components, ChartReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
	{
		var session = new ChartReadingSession(new() { Instruments = components }, config, formatting);
		using var reader = new ChartFileReader(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return CreateInstrumentSetFromReader(reader);
	}
	#endregion

	#region Metadata
	/// <summary>
	/// Reads metadata from a chart file.
	/// </summary>
	public static Metadata ReadMetadata(ReadingDataSource source, Metadata? existing = null)
	{
		var session = new ChartReadingSession(new() { Metadata = true }, DefaultReadConfig, null);
		using var reader = new ChartFileReader(source, session);

		reader.ExistingMetadata = existing;
		reader.Read();
		return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
	}

	public static async Task<Metadata> ReadMetadataAsync(ReadingDataSource source, Metadata? existing = null, CancellationToken cancellationToken = default)
	{
		var session = new ChartReadingSession(new() { Metadata = true }, DefaultReadConfig, null);
		using var reader = new ChartFileReader(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		reader.ExistingMetadata = existing;
		return reader.Parsers.TryGetFirstOfType(out MetadataParser? parser) ? parser!.Result : new();
	}
	#endregion

	#region Global events
	public static List<GlobalEvent> ReadGlobalEvents(ReadingDataSource source)
	{
		var session = new ChartReadingSession(new() { GlobalEvents = true }, DefaultReadConfig, null);
		using var reader = new ChartFileReader(source, session);

		reader.Read();
		return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : [];
	}

	public static async Task<List<GlobalEvent>> ReadGlobalEventsAsync(ReadingDataSource source, CancellationToken cancellationToken = default)
	{
		var session = new ChartReadingSession(new() { GlobalEvents = true }, DefaultReadConfig, null);
		using var reader = new ChartFileReader(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return reader.Parsers.TryGetFirstOfType(out GlobalEventParser? parser) ? parser!.Result! : [];
	}
	#endregion

	#region Vocals
	public static StandardVocalsTrack ReadVocals(ReadingDataSource source)
	{
		ReadGlobalEvents(source).GetLyrics(out var phrases, out var notes);
		return new(phrases, notes);
	}

	public static async Task<StandardVocalsTrack> ReadVocalsAsync(ReadingDataSource source, CancellationToken cancellationToken = default)
	{
		(await ReadGlobalEventsAsync(source, cancellationToken).ConfigureAwait(false)).GetLyrics(out var phrases, out var notes);
		return new(phrases, notes);
	}
	#endregion

	#region Sync track
	public static SyncTrack ReadSyncTrack(ReadingDataSource source, ChartReadingConfiguration? config = default)
	{
		var session = new ChartReadingSession(new() { SyncTrack = true }, config, null);
		using var reader = new ChartFileReader(source, session);

		reader.Read();
		return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
	}

	public static async Task<SyncTrack> ReadSyncTrackAsync(
		ReadingDataSource source, ChartReadingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		var session = new ChartReadingSession(new() { SyncTrack = true }, config, null);
		using var reader = new ChartFileReader(source, session);

		await reader.ReadAsync(cancellationToken).ConfigureAwait(false);
		return reader.Parsers.TryGetFirstOfType(out SyncTrackParser? syncTrackParser) ? syncTrackParser!.Result! : new();
	}
	#endregion
	#endregion

	#region Writing
	private static void FillInstrumentsWriterData(
		InstrumentSet set, InstrumentComponentList components, ChartWritingSession session, List<Serializer<string>> serializers, List<string> removedHeaders)
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

	#region Song
	private static ChartFileWriter GetSongWriter(WritingDataSource source, Song song, ComponentList components, ChartWritingSession session)
	{
		var removedHeaders = new List<string>();
		var serializers = new List<Serializer<string>>();

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
			// TODO Add <remark> that enabling the vocals component will also rewrite global events.
			if (components.Vocals)
			{
				var vocals = (song.Vocals ??= new()).Standard;

				song.GlobalEvents = (song.GlobalEvents is null
					? vocals.ToGlobalEvents()
					: song.GlobalEvents.SetLyrics(vocals)).ToList();
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

	public static void WriteSong(string path, Song song, ChartWritingConfiguration? config = default)
		=> WriteSong(new WritingDataSource(path), song, config);

	public static void WriteSong(Stream stream, Song song, ChartWritingConfiguration? config = default)
		=> WriteSong(new WritingDataSource(stream), song, config);

	/// <summary>
	/// Writes a song to a chart file.
	/// </summary>
	/// <param name="song">Song to write</param>
	public static void WriteSong(WritingDataSource source, Song song, ChartWritingConfiguration? config = default)
	{
		using var writer = GetSongWriter(source, song, ComponentList.Full(), new(config, song.Metadata?.Formatting));
		writer.Write();
	}

	public static Task WriteSongAsync(string path, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
		=> WriteSongAsync(new WritingDataSource(path), song, config, cancellationToken);

	public static Task WriteSongAsync(Stream stream, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
		=> WriteSongAsync(new WritingDataSource(stream), song, config, cancellationToken);

	public static async Task WriteSongAsync(WritingDataSource source, Song song, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		using var writer = GetSongWriter(source, song, ComponentList.Full(), new(config, song.Metadata?.Formatting));
		await writer.WriteAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion

	#region Components
	public static void ReplaceComponents(WritingDataSource source, Song song, ComponentList components, ChartWritingConfiguration? config = default)
	{
		using var writer = GetSongWriter(source, song, components, new(config, song.Metadata?.Formatting));
		writer.Write();
	}

	public static async Task ReplaceComponentsAsync(WritingDataSource source, Song song, ComponentList components, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		using var writer = GetSongWriter(source, song, components, new(config, song.Metadata?.Formatting));
		await writer.WriteAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion

	#region Instruments
	private static ChartFileWriter GetInstrumentsWriter(WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingSession session)
	{
		var serializers = new List<Serializer<string>>();
		var removedHeaders = new List<string>();

		FillInstrumentsWriterData(set, components, session, serializers, removedHeaders);

		return new(source, removedHeaders, [.. serializers]);
	}

	public static void ReplaceInstruments(WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingConfiguration? config = default, FormattingRules? formatting = default)
	{
		using var writer = GetInstrumentsWriter(source, set, components, new(config, formatting));
		writer.Write();
	}

	public static async Task ReplaceInstrumentsAsync(WritingDataSource source, InstrumentSet set, InstrumentComponentList components, ChartWritingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default)
	{
		using var writer = GetInstrumentsWriter(source, set, components, new(config, formatting));
		await writer.WriteAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion

	#region Metadata
	private static ChartFileWriter GetMetadataWriter(WritingDataSource source, Metadata metadata) => new(source, null, new MetadataSerializer(metadata));

	/// <summary>
	/// Replaces the metadata in a file.
	/// </summary>
	/// <param name="metadata">Metadata to write</param>
	public static void ReplaceMetadata(WritingDataSource source, Metadata metadata)
	{
		using var writer = GetMetadataWriter(source, metadata);
		writer.Write();
	}

	public static async Task ReplaceMetadataAsync(WritingDataSource source, Metadata metadata, CancellationToken cancellationToken = default)
	{
		using var writer = GetMetadataWriter(source, metadata);
		await writer.WriteAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion

	#region Global events
	private static ChartFileWriter GetGlobalEventWriter(WritingDataSource source, IEnumerable<GlobalEvent> events, ChartWritingSession session) => new(source, null, new GlobalEventSerializer(events, session));

	/// <summary>
	/// Replaces the global events in a file.
	/// </summary>
	/// <param name="path">Path of the file to write</param>
	/// <param name="events">Events to use as a replacement</param>
	public static void ReplaceGlobalEvents(WritingDataSource source, IEnumerable<GlobalEvent> events)
	{
		using var writer = GetGlobalEventWriter(source, events, new(DefaultWriteConfig, null));
		writer.Write();
	}

	public static async Task ReplaceGlobalEventsAsync(
		WritingDataSource source, IEnumerable<GlobalEvent> events, CancellationToken cancellationToken = default)
	{
		using var writer = GetGlobalEventWriter(source, events, new(DefaultWriteConfig, null));
		await writer.WriteAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion

	private static ChartFileWriter GetSyncTrackWriter(WritingDataSource source, SyncTrack syncTrack, ChartWritingSession session)
		=> new(source, null, new SyncTrackSerializer(syncTrack, session));

	/// <summary>
	/// Replaces the sync track in a file.
	/// </summary>
	/// <param name="path">Path of the file to write</param>
	/// <param name="syncTrack">Sync track to write</param>
	/// <param name="config"><inheritdoc cref="ReadingConfiguration" path="/summary"/></param>
	public static void ReplaceSyncTrack(WritingDataSource source, SyncTrack syncTrack, ChartWritingConfiguration? config = default)
	{
		using var writer = GetSyncTrackWriter(source, syncTrack, new(config, null));
		writer.Write();
	}

	public static async Task ReplaceSyncTrackAsync(
		WritingDataSource source, SyncTrack syncTrack, ChartWritingConfiguration? config = default, CancellationToken cancellationToken = default)
	{
		using var writer = GetSyncTrackWriter(source, syncTrack, new(config, null));
		await writer.WriteAsync(cancellationToken).ConfigureAwait(false);
	}
	#endregion
}
