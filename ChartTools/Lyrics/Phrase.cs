using ChartTools.Events;
using ChartTools.Extensions.Linq;

namespace ChartTools.Lyrics;

/// <summary>
/// Grouping of a <see cref="Lyrics.PhraseMarker"/> and set of <see cref="VocalsNote"/> for assembling lyric text.
/// </summary>
/// <param name="marker"></param>
/// <param name="notes"></param>
public class Phrase(PhraseMarker marker, IReadOnlyList<VocalsNote>? notes = null) : ILongTrackObject
{
	/// <summary>
	/// Marker defining the position and length of the phrase.
	/// </summary>
	public PhraseMarker PhraseMarker { get; } = marker;

	/// <summary>
	/// Set of vocals notes containing the pitches and text for the phrase.
	/// </summary>
	public IReadOnlyList<VocalsNote> Notes { get; } = notes ?? [];

	/// <summary>
	/// Start position of the phrase
	/// </summary>
	public uint Position
	{
		get => PhraseMarker.Position;
		set => PhraseMarker.Position = value;
	}

	/// <inheritdoc cref="PhraseMarker.Length"/>
	public uint Length
	{
		get => PhraseMarker.Length;
		set => PhraseMarker.Length = value;
	}

	/// <summary>
	/// Concatenated raw text of all notes in the phrase, separated by spaces.
	/// </summary>
	public string RawText
		=> BuildText(n => n.RawText);

	/// <summary>
	/// Phrase text assembled to its in-game appearance
	/// </summary>
	public string DisplayedText
		=> BuildText(static n => n.DisplayedText);

	private string BuildText(Func<VocalsNote, string> textSelector)
		=> string.Concat(
			Notes.Select(n => n.IsWordEnd ? textSelector(n) + ' ' : textSelector(n)));

	/// <summary>
	/// Converts the phrase to a set of global events representing the lyric data.
	/// </summary>
	/// <returns>Set of events generated during enumeration.</returns>
	public IEnumerable<GlobalEvent> ToGlobalEvents()
	{
		yield return new(Position, EventTypeHelper.Global.PhraseStart);

		foreach (VocalsNote note in Notes)
			yield return new(note.Position, EventTypeHelper.Global.Lyric, note.RawText);

		if (PhraseMarker.Length > 0)
			yield return new((PhraseMarker as ILongTrackObject).EndPosition, EventTypeHelper.Global.PhraseEnd);
	}
}

/// <summary>
/// Provides additional methods to <see cref="Phrase"/>.
/// </summary>
public static class PhraseExtensions
{
	public static void GetLyrics(
		this IEnumerable<GlobalEvent> events, out IList<PhraseMarker> phrases, out IList<VocalsNote> notes)
	{
		PhraseMarker? phrase = null;

		phrases = [];
		notes   = [];

		foreach (GlobalEvent e in events.OrderBy(static e => e.Position))
		{
			switch (e.EventType)
			{
				case EventTypeHelper.Global.PhraseStart:
					phrase = new(e.Position);
					phrases.Add(phrase);
					break;
				case EventTypeHelper.Global.Lyric:
					notes.Add(new(e.Position, VocalsPitchValue.None, e.Argument));
					break;
				case EventTypeHelper.Global.PhraseEnd:
					phrase?.Length = e.Position - phrase.Position;
					break;
			}
		}
	}

	public static IEnumerable<Phrase> GetLyrics(IEnumerable<PhraseMarker> phrases, IEnumerable<VocalsNote> notes)
	{
		using IEnumerator<PhraseMarker> phraseEnumerator = phrases
			.OrderBy(static p => p.Position).GetEnumerator();

		if (!phraseEnumerator.MoveNext())
			yield break;

		using IEnumerator<VocalsNote> notesEnumerator = notes
			.OrderBy(static n => n.Position).GetEnumerator();

		notesEnumerator.MoveNext(); // Initialize prematurely to simplify the loop flow

		PhraseMarker lastMarker = phraseEnumerator.Current;
		List<VocalsNote> lastPhraseNotes = [];

		// Keeps track on if the notes enumerator reached the end as IEnumerator provides no safe way of checking without mutating
		bool notesRemaining = true;

		while (phraseEnumerator.MoveNext()) // Peek forward and add prior notes to the last phrase
		{
			while (notesRemaining && notesEnumerator.Current.Position < phraseEnumerator.Current.Position)
			{
				lastPhraseNotes.Add(notesEnumerator.Current);
				notesRemaining = notesEnumerator.MoveNext();
			}

			yield return new(lastMarker, lastPhraseNotes);
			lastMarker = phraseEnumerator.Current;

			lastPhraseNotes.Clear();
		}

		// Add remaining notes to the last phrase
		while (notesRemaining)
		{
			lastPhraseNotes.Add(notesEnumerator.Current);
			notesRemaining = notesEnumerator.MoveNext();
		}

		yield return new(lastMarker, lastPhraseNotes);
	}

	public static IEnumerable<Phrase> GetLyrics(this StandardVocalsTrack track)
		=> GetLyrics(track.Phrases, track.Notes);

	/// <summary>
	/// Wraps lyrics-related global events into a set of grouped phrases for easier handling.
	/// </summary>
	/// <param name="events">Set of events</param>
	public static IEnumerable<Phrase> GetLyrics(this IEnumerable<GlobalEvent> events)
	{
		GetLyrics(events, out IList<PhraseMarker> phrases, out IList<VocalsNote> notes);
		return GetLyrics(phrases, notes);
	}

	public static IEnumerable<GlobalEvent> ToGlobalEvents(IEnumerable<PhraseMarker> markers, IEnumerable<VocalsNote> notes)
	{
		foreach (PhraseMarker marker in markers)
		{
			yield return new(marker.Position, EventTypeHelper.Global.PhraseStart);

			if (marker.Length > 0)
				yield return new((marker as ILongTrackObject).EndPosition, EventTypeHelper.Global.PhraseEnd);
		}

		foreach (VocalsNote note in notes)
			yield return new(note.Position, EventTypeHelper.Global.Lyric, note.RawText);
	}

	/// <summary>
	/// Converts a set of <see cref="Phrase"/> to a set of <see cref="GlobalEvent"/> making up the phrases.
	/// </summary>
	/// <param name="source">Phrases to convert into global events</param>
	/// <returns>Global events making up the phrases</returns>
	public static IEnumerable<GlobalEvent> ToGlobalEvents(this IEnumerable<Phrase> source)
		=> source.SelectMany(static p => p.ToGlobalEvents());

	public static IEnumerable<GlobalEvent> SetLyrics(
		this IEnumerable<GlobalEvent> events, IEnumerable<PhraseMarker> markers, IEnumerable<VocalsNote> notes)
	{
		IEnumerable<GlobalEvent>[] collections =
		[
		   events.Where(static e => !e.IsLyricEvent),
		   ToGlobalEvents(markers, notes)
		];

		return collections.AlternateBy(static e => e.Position);
	}

	/// <summary>
	/// Generates a new set of global events where lyric-related events are replaced with ones matching a set of phrases.
	/// </summary>
	/// <param name="events">Original set of events</param>
	/// <param name="phrases">Phrases containing the new lyric data</param>
	/// <returns>New set of events generated during enumeration.</returns>
	/// <remarks>Non-lyric events are maintained object-wise across the old and new set.</remarks>
	public static IEnumerable<GlobalEvent> SetLyrics(this IEnumerable<GlobalEvent> events, IEnumerable<Phrase> phrases)
	{
		IEnumerable<GlobalEvent>[] collections =
		[
			events.Where(static e => !e.IsLyricEvent),
			phrases.ToGlobalEvents()
		];

		return collections.AlternateBy(static e => e.Position);
	}

	/// <summary>
	/// Generates a new set of global events where lyric-related events are replaced with ones matching a vocals track.
	/// </summary>
	/// <param name="events">Original set of events</param>
	/// <param name="track">Vocals track containing the new lyric data</param>
	/// <inheritdoc cref="SetLyrics(IEnumerable{GlobalEvent}, IEnumerable{Phrase})"/>
	public static IEnumerable<GlobalEvent> SetLyrics(this IEnumerable<GlobalEvent> events, StandardVocalsTrack track)
		=> events.SetLyrics(track.Phrases, track.Notes);
}
