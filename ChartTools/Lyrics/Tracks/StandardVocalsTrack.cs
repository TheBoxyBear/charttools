using ChartTools.Events;

namespace ChartTools.Lyrics.Tracks;

public class StandardVocalsTrack(IList<PhraseMarker>? phrases = null, IList<VocalsNote>? notes = null) : VocalsTrack(phrases)
{
	public IList<VocalsNote> Notes { get; } = notes ?? [];

	public IEnumerable<GlobalEvent> ToGlobalEvents() => PhraseExtensions.ToGlobalEvents(Phrases, Notes);
}
