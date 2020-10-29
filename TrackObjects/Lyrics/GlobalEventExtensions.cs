using ChartTools.Collections.Alternating;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Provides additionnal methods for <see cref="GlobalEvent"/>
    /// </summary>
    public static class GlobalEventExtensions
    {
        /// <summary>
        /// Gets the lyrics from an enumerable of <see cref="GlobalEvent"/>
        /// </summary>
        /// <returns>Enumerable of <see cref="Phrase"/></returns>
        public static IEnumerable<Phrase> GetLyrics(this IEnumerable<GlobalEvent> globalEvents)
        {
            Phrase phrase = null;
            Syllable phraselessFirstSyllable = null;

            foreach (GlobalEvent globalEvent in globalEvents.OrderBy(e => e.Position))
                switch (globalEvent.EventType)
                {
                    case GlobalEventType.PhraseStart:
                        if (phrase is not null)
                            yield return phrase;

                        phrase = new Phrase(globalEvent.Position);

                        //If the stored lyric has the same position as the new phrase, add it to the phrase
                        if (phraselessFirstSyllable is not null && phraselessFirstSyllable.Position == globalEvent.Position)
                        {
                            phrase.Syllables.Add(phraselessFirstSyllable);
                            phraselessFirstSyllable = null;
                        }
                        break;
                    case GlobalEventType.Lyric:
                        Syllable newSyllable = new Syllable(globalEvent.Position) { RawText = globalEvent.Argument };

                        //If the first lyric preceeds the first phrase, store it
                        if (phrase is null)
                            phraselessFirstSyllable = newSyllable;
                        else
                            phrase.Syllables.Add(newSyllable);
                        break;
                    case GlobalEventType.PhraseEnd:
                        if (phrase is not null)
                            phrase.EndPosition = globalEvent.Position;
                        break;
                }

            if (phrase is not null)
                yield return phrase;
        }
        /// <summary>
        /// Gets a set of <see cref="GlobalEvent"/> where phrase and lyric events are replaced with the events makign up a set of <see cref="Phrase"/>.
        /// </summary>
        /// <returns>Enumerable of <see cref="GlobalEvent"/></returns>
        public static IEnumerable<GlobalEvent> SetLyrics(this IEnumerable<GlobalEvent> events, IEnumerable<Phrase> lyrics)
        {
            foreach (GlobalEvent globalEvent in new OrderedAlternatingEnumerable<GlobalEvent, uint>(i => i.Position, events.Where(e => !e.IsLyricEvent), lyrics.SelectMany(p => p.ToGlobalEvents())))
                yield return globalEvent;
        }
    }
}
