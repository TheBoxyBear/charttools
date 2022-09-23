# Lyrics
Lyrics of a song are defined by a collection of phrases. A phrase represents a single line of lyrics, which are split up into syllables.

Phrases can either be read from a file:

```csharp
using ChartTools.Lyrics;

IEnumerable<Phrase> lyrics = Phrase.FromFile(path);
```

or using existing global events:

```csharp
using ChartTools.Lyrics;

IEnumerable<Phrase> lyrics = song.GlobalEvents.GetLyrics();
```

To be written to a file, lyrics must be converted back into global events:

```csharp
using ChartTools.Lyrics;

lyrics.ToGlobalEvents(); // Creates a new set of global events
events.SetLyrics(lyrics); // Replaces existing lyric-related events with new events making up the phrases
```

It is also possible to edit lyrics directly from the global events. The use of phrase and syllable objects are intended to simplify the editing of lyrics, and any changes to these objects are only applied to the song once they are converted back into global events.
