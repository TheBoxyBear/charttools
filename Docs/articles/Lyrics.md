# Lyrics
Lyrics of a song are defined by the Vocals instrument in which chords are phrases and notes as syllables. Unlike with lane chords which are defined by notes at the same position, syllables in phrases are consecutive and arbitrarily grouped into phrases. Lyric classes are defined under the `ChartTools.Lyrics` namespace.

Although Clone Hero does not support vocals as an instrument and typically stores lyric data as global events, the `ChartTools.Lyrics` namespace provides a more rigorous API for editing lyrics. ChartTools also provides methods to convert between global events and vocals. Clone Hero also uses vocals to read lyrics from legacy songs.

## Syllables
Syllables define a start and end position in the form of offsets from the start position of the parent phrase. This allows for phrases to be moved by changing their position without having to update the position of each syllable.

The note index of syllables represents a vocal pitch from the range C2 to C6 using the `VocalsPitchValue`, stored under the `VocalPitch` helper struct. The enum uses a binary representation to isolate keys and octaves while staying true to music theory with comparing values.

The following code creates a syllable with the pitch of D3.

```csharp
Phrase phrase = song.Instruments.Vocals.Expect.Phrases;

phrase.Syllables.Add(new Syllable(0, new VocalsPitch(3 << 4 | VocalsPitch.D));
```

Vocals also store lyric text to appear in-game. The text for each phrase is divided between the syllables to define the timings of the karaoke system. Clone Hero defines special characters such as dashes for multi-syllable words that don't appear in-game. The raw text with special characters can be access through the `RawText` property while `DisplayedText` processes the special characters and returns the syllable text as it appears in-game.

> **NOTE**: Clone Hero also supports color data in the text. This is currently not supported in ChartTools and special characters related to color data will appear as part of `DisplayedText`.

## Phrases
Unlike other chords, phrases define a length and end position in addition to a start position. The end position is driven by the end of the last syllable but can be replaced by setting the `LengthOverride` property. Phrases also define `RawText` and `DisplayedText` properties which combine the text from their syllables.

## Reading and writing vocals
Vocals can be read like any other instrument using the `FromFile` method.

```csharp
Vocals? vocals = Vocals.FromFile(path, <ReadingConfiguration>, <FormattingRules>);
```

Chart files define lyrics through global events that can be converted to a set of phrases. When reading a full song from a chart file, the vocals insturment will be null.

```csharp
using ChartTools.Lyrics;

IEnumerable<Phrase> lyrics = song.GlobalEvents.GetLyrics();
```

To be written to a chart file, lyrics must be converted back into global events.

```csharp
using ChartTools.Lyrics;

lyrics.ToGlobalEvents(); // Creates a new set of global events
events.SetLyrics(lyrics); // Replaces existing lyric-related events with new events making up the phrases
```

It is also possible to edit lyrics directly from the global events, although syllable objects simplifies the editing process. When doing so, phrases must be converted back into events for the changes to be applied.