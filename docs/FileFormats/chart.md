# Chart

Chart is a customs format originating from the GH1/2 era. It is text-based and uses the `.chart` extension.

## Sections

### Basic info

Data in .chart files is grouped into sections. Sections start with their name in square brackets, and follow with their contents encapsulated by curly brackets. Section names are always written in PascalCase. Sections do not have to be separated with line breaks.

```
[SectionOne]
{
    // Section contents go here
}
[SectionTwo]
{
    // Section contents go here
}
```

### Section names

The first sections are:

- `Song` – Song metadata
- `SyncTrack` – Time signatures and tempo markers
- `Events` – Global events

Following these, each difficulty of every instrument is its own section, containing various track objects detailed later. The nomenclature of these sections is the name of the difficulty followed by a code name for the instrument. For example, Hard on Drums is written as `HardDrums`.

The difficulty names are:

- `Expert`
- `Hard`
- `Medium`
- `Easy`
  
The instrument names are:

- `Single` – Lead guitar
- `DoubleGuitar` – Co-op guitar
- `DoubleBass` – Bass
- `DoubleRhythm` – Rhythm guitar
- `Keyboard` – Keyboard
- `Drums` – Drums
- `GHLGuitar` – Guitar Hero Live Guitar
- `GHLBass` – Guitar Hero Live Bass
- (`SingleBass` may also be present in older charts and was supported by Feedback Editor, but this was never used in any games and is considered to be unsupported.)

All instrument sections are optional. A missing section is equivalent to a section with no data.

### Section data

Section data is represented by objects, each one separated by a line break. Section objects are written as key-value pairs:

`<Key> = <Value>`

The key is a string in the Song section, and a number everywhere else. The value type is either a string or a number, depending on what the key is. Numbers are always written in base 10.

## Metadata

### Song metadata

The `Song` section contains metadata about the song and chart. Each info entry is identified by a key, written in PascalCase, with the info stored as the object data. String data is surrounded by quotation marks while numerical data.

The order of metadata objects may vary, so you should *not* rely on the order to identify each object, and should only rely on their keys.

This section is not used by Clone Hero for metadata (except for the chart resolution), it instead gets metadata from an accompanying song.ini file. The only time CH uses Chart metadata is if a song.ini does not exist, in which case it will create one with that metadata.

List of available metadata (except audio streams, those are included in the next section):

| `Key`          | Description                                                                                                | Data type |
| :---:          | :---                                                                                                       | :---:     |
| `Name`         | Title of the song.                                                                                         | string    |
| `Artist`       | Artist or band behind the song.                                                                            | string    |
| `Charter`      | Community member responsible for charting the song.                                                        | string    |
| `Album`        | Title of the album the song is featured in.                                                                | string    |
| `Genre`        | Genre of the song.                                                                                         | string    |
| `Year`         | Year of the song’s release. Should be preceded by a comma and a space, e.x. `, 2002`.                      | string    |
| `Offset`       | Start time of the audio in seconds. A higher value makes the audio start sooner. This is not recommended to be used by today’s charting standards, and is maintained mostly for backwards compatibility. | number |
| `Resolution`   | Number of positional ticks between the start of a beat and the next. Default is 192.                       | number    |
| `Player2`      | Name of the player 2 instrument.                                                                           | string    |
| `Difficulty`   | Estimated difficulty of the song.                                                                          | number    |
| `PreviewStart` | Time of the song, in milliseconds, where the song preview starts.                                          | number    |
| `PreviewEnd`   | Time of the song, in milliseconds, where the song preview ends.                                            | number    |
| `MediaType`    | Media type for the audio source.                                                                           | string    |

### Audio streams

Streams are paths to audio files to play during the song. These paths are relative to the location of the chart file. Some files are specific to an instrument and may be muted depending on the performance of the player behind the instrument.

These stream paths are not used by Clone Hero, it instead has static names for these tracks, and dynamically includes them based on their presence.

 | `Key`          | Description                                                                                     | Data type |
 | :---:          | :---                                                                                            | :---:     |
 | `MusicStream`  | Contains either the entire song, or any audio that doesn't fit into the other streams.          | string    |
 | `GuitarStream` | Contains the lead guitar audio.                                                                 | string    |
 | `RhythmStream` | Contains the rhythm guitar audio.                                                               | string    |
 | `BassStream`   | Contains the bass audio.                                                                        | string    |
 | `KeysStream`   | Contains keyboard/synth audio.                                                                  | string    |
 | `DrumStream`   | Contains the audio for the kick drum, and the contents of the next streams if they are absent.  | string    |
 | `Drum2Stream`  | Contains the audio for the snare drum, and the contents of the next streams if they are absent. | string    |
 | `Drum3Stream`  | Contains the audio for toms, and the contents of the next stream if it is absent.               | string    |
 | `Drum4Stream`  | Contains the audio for cymbals.                                                                 | string    |
 | `VocalStream`  | Contains the vocals audio.                                                                      | string    |
 | `CrowdStream`  | Contains crowd singing audio.                                                                   | string    |

## Track Objects and Type Codes

Track objects are objects with a numerical position, and are written like this:

`<Position> = <Type> <Value[]>`

The position is used as the key. Types are a string (typically a single character), and values are numbers. Different event types have different amounts of values. Track objects in the same section should be written in increasing order of position, and for events on the same position, they should be sorted first by type (alphabetically), then by value (numerically).

Some older charts may have some objects out of order, and Clone Hero v.23 and earlier can play songs that have out-of-order objects, but the Public Test Build marks these songs as bad songs due to issues they can cause.

### Time signatures

Time signature markers are SyncTrack objects and use the `TS` type code.

`TS <Numerator> <Denominator exponent>`

`Numerator` is the numerator to use for the time signature.

`Denominator exponent` is the power of 2 to use for the denominator of the time signature. (Optional, defaults to 2 if unspecified.) In other words: Denominator = `2 ^ <denominator exponent>`

Examples:

4/4 is `TS 4` or `TS 4 2`.

7/16 is `TS 7 4`.

3/8 is `TS 3 3`.

### Tempo

Tempo markers are SyncTrack objects and use the `B` type code.

`B <Tempo * 1000>`

`Tempo * 1000` is the BPM multiplied by 1000, and is stored as a 6-digit number. The first 3 digits are a whole value, the last 3 digits are a decimal value. Any trailing zeroes should be removed.

Examples:

120 BPM = `B 120000`

60 BPM = `B 60000`

150.325 BPM = `B 150325`

### Tempo anchors

Anchor markers are SyncTrack objects and use the `A` type code. They lock a tempo marker's position to a time position relative to the audio. The preceding tempo marker should have its tempo adjusted to maintain this lock if any of the preceding tempo markers are adjusted.

`A <Audio time position>`

`Audio time position` is the position that the associated tempo marker should be set to, in microseconds.

Example:

This will anchor a 120 BPM tempo marker at position 864 to the time position at 2.25 seconds.

```
864 = A 2250000
864 = B 120000
```

### Events

Events use the `E` type code. Its data is written as a string.

`E <Event>`

`Event` is the event data.

Events can appear in both the `Events` track (global events), and in any of the instrument tracks (local events).

Global event data is surrounded by quotation marks, and as a result, having quotation marks in them is disallowed. Some charts work around this however by using `/"` or `"/"` if it goes before a syllable, and `"/` if it goes after. The CH PTB is able to parse quotation marks in events without these workarounds, as well.

Local events are written plainly with no surrounding, and cannot have spaces in them. There may be .chart files out there with spaces in local events, though.

Common global event types:

- `phrase_start` – Start of a new lyrics phrase. Marks the end of the previous phrase if it has no end event.
- `phrase_end` – End of the current lyrics phrase. Lyrics will disappear at this event if far enough from the next start event.
- `lyric <syllable>` – Stores a syllable of the current lyric phrase as the argument. All of the lyrics within a phrase get concatenated into a single phrase in-game. Text gets highlighted one syllable at a time by the lyrics system as lyric events are passed. There are various symbols that do various things or get stripped out:
  - Hyphens `-` will combine two syllables into a single word.
  - Equals `=` will join two symbols using a hyphen.
  - Underscores `_`, pluses `+`, hashes `#`, and carets `^` are stripped out by CH. Dollars `$` and sections `§` probably should be as well, but they currently aren't.
  - CH's PTB can properly parse [certain](https://strikeline.myjetbrains.com/youtrack/issue/CH-226) [TextMeshPro formatting tags](http://digitalnativestudios.com/textmeshpro/docs/rich-text/), such as `<color=#00FF00>`.
  - Syllables not joined together through anything will automatically be separated by a space.
- `section <name>` – Marks a new section used by Practice mode and post-game summary.
- `end` – Marks the end of a song.

Common local event types:

- `solo` - Starts a solo.
- `soloend` - Ends a solo.
- `mix_<difficulty>_drums0d` - Enables disco flip on drums. For `Difficulty`, 0 is Expert, 3 is Easy, etc.
- `mix_<difficulty>_drums0` - Disables disco flip on drums.

### Notes, modifiers/flags, and Star Power

Notes and modifiers are instrument track objects and use the `N` type code.

`N <Type> <Length>`

`Type` is the type of note/modifier that this object represents. Length is used for the length of the note/modifier.

`Length` is the length of a note. This value doesn't do anything for modifiers.

As with tempo anchors, modifiers are applied to notes but are listed as separate objects. A modifier applies to all notes in the same position, so different notes of the same position cannot have different modifiers unless the modifier specifically targets a single color/type.

To help prevent issues such as Black 3 in the GHL notes having a non-sequential value of 8, moving forward the types will be divided into groups of 32:

- 0 through 31 for standard GH notes
- 32 through 63 for GH-specific modifiers such as Expert+ kicks
- 64 through 95 for Rock Band-specific stuff such as cymbal markers
- 96 through 127 for any CH-specific stuff

Any games that want to add custom note/modifier types should reserve the next set of 32.

5-fret instrument note types:

- 0 – Green note
- 1 – Red note
- 2 – Yellow note
- 3 – Blue note
- 4 – Orange note
- 5 – Strum/HOPO flip (force) modifier
- 6 – Tap modifier
- 7 – Open note

Guitar Hero Live instrument note types:

- 0 – White 1 note
- 1 – White 2 note
- 2 – White 3 note
- 3 – Black 1 note
- 4 – Black 2 note
- 5 – Strum/HOPO flip (force) modifier
- 6 – Tap modifier
- 7 – Open note
- 8 – Black 3 note

Drums note types:

- 0 – Kick
- 1 – Red
- 2 – Yellow
- 3 – Blue
- 4 – 5-lane Orange, 4-lane Green
- 5 – 5-lane Green
- 32 – 2x kick
- 66 – Yellow cymbal modifier
- 67 – Blue cymbal modifier
- 68 – Green cymbal modifier

### Special

Special objects are instrument track objects and use the `S` type code. These are other track-specific objects that do not fit under the description of notes/modifiers.

`S <Type> <Length>`

Types:

- 0 - GH1/2 Co-op primary player
- 1 - GH1/2 Co-op secondary player
- 2 - Star power
- 64 - Drums SP activation phrase

## Notes

A large part of this info comes from [Firefox's .chart specifications](https://docs.google.com/document/d/1v2v0U-9HQ5qHeccpExDOLJ5CMPZZ3QytPmAG5WF0Kzs/edit#).
