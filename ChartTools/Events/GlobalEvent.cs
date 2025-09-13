using ChartTools.IO;
using ChartTools.IO.Chart;

namespace ChartTools.Events;

/// <summary>
/// Event common to all instruments
/// </summary>
public class GlobalEvent : Event
{
	/// <summary>
	/// The event controls movement of the bassist character on stage
	/// </summary>
	public bool IsBassistMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.BassistMovement);

	/// <summary>
	/// The event controls movement of the crowd
	/// </summary>
	public bool IsCrowdEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Crowd);

	/// <summary>
	/// The event controls movement of the drummer character on stage
	/// </summary>
	public bool IsDrummerMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.DrummerMovement);

	/// <summary>
	/// The event controls movement of the guitarist character on stage
	/// </summary>
	public bool IsGuitaristMovementEvent =>
		EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristMovement) ||
		EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristSolo);

	public bool IsGuitaristSoloEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristSolo);

	/// <summary>
	/// The event controls movement of the keyboard player character on stage
	/// </summary>
	public bool IsKeyboardMovementEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.KeyboardMovement);

	/// <summary>
	/// The event stores text for a syllable of lyrics for karaoke when no vocal track is present
	/// </summary>
	public bool IsLyricEvent => IsPhraseEvent || EventType == EventTypeHelper.Global.Lyric;

	/// <summary>
	/// The event represents the start or end of a lyric phrase when no vocal track is present
	/// </summary>
	public bool IsPhraseEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Phrase);

	/// <summary>
	/// The event represents a section of a song used for post-game summary and practice mode
	/// </summary>
	public bool IsSectionEvent => EventType is EventTypeHelper.Global.RB2CHSection or EventTypeHelper.Global.RB3Section;

	public bool IsSyncEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.Sync);

	public bool IsWailEvent => EventType.StartsWith(EventTypeHeaderHelper.Global.GuitaristWail);

	/// <inheritdoc cref="Event(uint)"/>
	public GlobalEvent(uint position) : base(position) { }

	/// <inheritdoc cref="Event(uint, string)"/>
	public GlobalEvent(uint position, string data) : base(position, data) { }

	/// <inheritdoc cref="Event(uint, string, string)"/>
	public GlobalEvent(uint position, string type, string argument = "") : base(position, type, argument) { }

	/// <summary>
	/// Reads the set of <see cref="GlobalEvent"/> from a file.
	/// </summary>
	/// <param name="path">Path of the file to read from</param>
	public static IEnumerable<GlobalEvent> FromFile(string path)
		=> ExtensionHandler.Read<IEnumerable<GlobalEvent>>(path, (".chart", p => ChartFile.ReadGlobalEvents(p)));

	/// <summary>
	/// Reads the set of <see cref="GlobalEvent"/> from a file asynchronously.
	/// </summary>
	/// <param name="path">Path of the file to read from</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static async Task<List<GlobalEvent>> FromFileAsync(string path, CancellationToken cancellationToken = default)
		=> await ExtensionHandler.ReadAsync(path, (".chart", path => ChartFile.ReadGlobalEventsAsync(path, cancellationToken)))
		.ConfigureAwait(false);
}

/// <summary>
/// Provides extensions methods for sets of <see cref="GlobalEvent"/>
/// </summary>
public static class GlobalEventExtensions
{
	/// <summary>
	/// Replaces the set of <see cref="GlobalEvent"/> in a file.
	/// </summary>
	/// <param name="events">Set of events to write</param>
	/// <param name="path">Path of the file to write to</param>
	public static void ToFile(this IEnumerable<GlobalEvent> events, string path)
		=> ExtensionHandler.Write(path, events, (".chart", (p, e) => ChartFile.ReplaceGlobalEvents(p, e)));

	/// <summary>
	/// Replaces the set of <see cref="GlobalEvent"/> in a file asynchronously.
	/// </summary>
	/// <param name="events">Set of events to write</param>
	/// <param name="path">Path of the file to write to</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static Task ToFileAsync(this IEnumerable<GlobalEvent> events, string path, CancellationToken cancellationToken = default)
		=> ExtensionHandler.WriteAsync(path, events, (".chart", (p, e) => ChartFile.ReplaceGlobalEventsAsync(p, e, cancellationToken)));
}
