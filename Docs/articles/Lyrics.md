# Lyrics
Lyrics of a song are defined by the Vocals instrument in which chords are phrases and notes as syllabkes. Unlike with lane chords which are defined by notes at the same position, syllables in phrases are consecutive and arbitrarily grouped into phrases.

## Syllables
Syllables define a start and end position in the form of offsets from the start position of the parent phrase. This allows for phrases to be moved by changing their position without having to update the position of each syllable.

The note index of syllables represents a vocal pitch from the range C2 to C6 using the `VocalPitchValue` enum. The enum defines each value so that the octave is defined in the upper four bits and the key in the lower four bits. This facilitates the definition of new values from a key and pitch, extracting such data from a pitch as well as ensuring that comparisons on the numerical value of the enum follow music theory. The pitch is stored in the syllable as a wrapper struct with helper properties for obtaining the key and pitch.

Vocals also store lyric text to appear in-game. The text for each phrase is divided between the syllables to define the timings of the karaoke system. Clone Hero defines special characters such as dashes for multi-syllable words that don't appear in-game. The raw text with special characters can be access through the `RawText` propterry while `DisplayedText` processes the special characters and returns the syllable text as it appears in-game.

> **NOTE**: Clone Hero also supports color data in the text. This is currently not supported in ChartTools and special characters related to color data will appear as part of `DisplayeText`.

## Phrases
Unlike other chords, phrases define a length and end position in addition to a start position. The end position is driven by the end of the last syllable but can be replced by setting the `LengthOverride` proeprty. Phrases also define a `RawText` and `DisplayedText` which combine the text from the syllables.

Phrases can either be read from a file.

## Reading and writing vocals
Vocals can be read like any other instrument using the `FromFile` method.

```csharp
Instrument<Phrase>? vocals = Instrument.FromFile(path, <ReadingConfiguration>, <FormattingRules>);
```

Chart files define lyrics through the global events. Lyrics can be obtained from an existing set of events. When reading a full song from a chart file, the vocals insturment will be null.

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

It is also possible to edit lyrics directly from the global events. The use of phrase and syllable objects are intended to simplify the editing of lyrics, and any changes to these objects are only applied to the song once they are converted back into global events.
