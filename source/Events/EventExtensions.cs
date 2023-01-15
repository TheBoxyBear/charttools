using ChartTools.Extensions.Collections;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.Lyrics;

namespace ChartTools.Events;

/// <summary>
/// Provides additional methods for events.
/// </summary>
public static class EventExtensions
{
    public static void ToFile(this IEnumerable<GlobalEvent> events, string path) => ExtensionHandler.Write(path, events, (".chart", (path, events) => ChartFile.ReplaceGlobalEvents(path, events)));
    public static async Task ToFileAsync(this IEnumerable<GlobalEvent> events, string path, CancellationToken cancellationToken) => await ExtensionHandler.WriteAsync(path, events, (".chart", (path, events) => ChartFile.ReplaceGlobalEventsAsync(path, events, cancellationToken)));

    /// <summary>
    /// Gets the lyrics from an enumerable of <see cref="GlobalEvent"/>
    /// </summary>
    /// <returns>Enumerable of <see cref="Phrase"/></returns>
    public static IEnumerable<Phrase> GetLyrics(this IEnumerable<GlobalEvent> globalEvents)
    {
        Phrase? phrase = null;

        foreach (GlobalEvent globalEvent in globalEvents.OrderBy(e => e.Position))
            switch (globalEvent.EventType)
            {
                // Change active phrase
                case EventTypeHelper.Global.PhraseStart:
                    if (phrase is not null)
                        yield return phrase;

                    phrase = new Phrase(globalEvent.Position);
                    break;
                // Add syllable to the active phrase using the event argument
                case EventTypeHelper.Global.Lyric:
                    phrase?.Syllables.Add(new(globalEvent.Position - phrase.Position, VocalPitchValue.None) { RawText = globalEvent.Argument ?? string.Empty });
                    break;
                // Set length of active phrase
                case EventTypeHelper.Global.PhraseEnd:
                    if (phrase is not null)
                        phrase.LengthOverride = globalEvent.Position - phrase.Position;
                    break;
            }

        if (phrase is not null)
            yield return phrase;
    }
    /// <summary>
    /// Gets a set of <see cref="GlobalEvent"/> where phrase and lyric events are replaced with the events making up a set of <see cref="Phrase"/>.
    /// </summary>
    /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
    public static IEnumerable<GlobalEvent> SetLyrics(this IEnumerable<GlobalEvent> events, IEnumerable<Phrase> lyrics)
    {
        foreach (GlobalEvent globalEvent in new OrderedAlternatingEnumerable<GlobalEvent, uint>(i => i.Position, events.Where(e => !e.IsLyricEvent), lyrics.SelectMany(p => p.ToGlobalEvents())))
            yield return globalEvent;
    }
}
