# Lyrics
Lyrics of a song are defined by the [Vocals](~/api/ChartTools.Vocals.yml) instrument in which chords are [phrases](~/api/ChartTools.Lyrics.Phrase.yml) and notes as [syllables](~/api/ChartTools.Lyrics.Syllable.yml). Unlike with lane chords which are defined by notes at the same position, syllables in phrases are consecutive and arbitrarily grouped into phrases. Lyric classes are defined under the [ChartTools.Lyrics](~/api/ChartTools.Lyrics.yml) namespace.

Although Clone Hero does not support vocals as an instrument and typically stores lyric data as global events, the [ChartTools.Lyrics](~/api/ChartTools.Lyrics.yml) namespace provides a more rigorous API for editing lyrics. ChartTools also provides methods to convert between global events and vocals. Clone Hero also uses vocals to read lyrics from legacy songs.

## Syllables
[Syllables](~/api/ChartTools.Lyrics.Syllable.yml) define a start and end position in the form of offsets from the start position of the parent phrase. This allows for phrases to be moved by changing their position without having to update the position of each syllable.

The note index of syllables represents a vocal pitch from the range C2 to C6 using the [VocalPitchValue](~/api/ChartTools.Lyrics.VocalPitchValue.yml), stored under the [VocalPitch](~/api/ChartTools.Lyrics.VocalsPitch.yml) helper struct. The enum uses a binary representation to isolate keys and octaves while staying true to music theory with comparing values.

The following code creates a syllable with the pitch of D3.

```csharp
Phrase phrase = song.Instruments.Vocals.Expect.Phrases;

phrase.Syllables.Add(new Syllable(0, new VocalsPitch(3 << 4 | VocalsPitch.D));
```

[Vocals](~/api/ChartTools.Vocals.yml) also store lyric text to appear in-game. The text for each phrase is divided between the syllables to define the timings of the karaoke system. Clone Hero defines special characters such as dashes for multi-syllable words that don't appear in-game. The raw text with special characters can be access through the [RawText](~/api/ChartTools.Lyrics.Syllable.yml#ChartTools_Lyrics_Syllable_RawText) property while [DisplayedText](~/api/ChartTools.Lyrics.Syllable.yml#ChartTools_Lyrics_Syllable_DisplayedText) processes the special characters and returns the syllable text as it appears in-game.

> **NOTE**: Clone Hero also supports color data in the text. This is currently not supported in ChartTools and special characters related to color data will appear as part of [DisplayedText](~/api/ChartTools.Lyrics.Syllable.yml#ChartTools_Lyrics_Syllable_DisplayedText).

## Phrases
Unlike other chords, [phrases](~/api/ChartTools.Lyrics.Phrase.yml) define a length and end position in addition to a start position. The end position is driven by the end of the last syllable but can be replaced by setting the [LengthOverride](~/api/ChartTools.Lyrics.Phrase.yml#ChartTools_Lyrics_Phrase_LengthOverride) property. Phrases also define [RawText](~/api/ChartTools.Lyrics.Syllable.yml#ChartTools_Lyrics_Phrase_RawText) and [DisplayedText](~/api/ChartTools.Lyrics.Syllable.yml#ChartTools_Lyrics_Phrase_DisplayedText) properties which combine the text from their syllables.

## Reading and writing vocals
Chart files define lyrics through global events that can be converted to a set of phrases. When reading a full song from a chart file, the vocals instrument will be null. To benefit of the more advanced lyrics API, a dummy set of phrases instrument can be generated using the [GetLyrics](~/api/ChartTools.Events.EventExtensions.yml#ChartTools_Events_EventExtensions_GetLyrics_System_Collections_Generic_IEnumerable_ChartTools_Events_GlobalEvent__) extension method.

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

