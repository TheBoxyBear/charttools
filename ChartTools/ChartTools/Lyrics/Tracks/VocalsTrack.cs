namespace ChartTools.Lyrics;

public abstract class VocalsTrack(IList<PhraseMarker>? markers = null)
{
	public IList<PhraseMarker> Phrases { get; } = markers ?? [];
}
