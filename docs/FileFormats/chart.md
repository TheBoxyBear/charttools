# Chart

Chart is a customs format originating from the GH1/2 era. It is text-based and uses the `.chart` file extension.

## Table of Contents

- [Basic Infrastructure](#basic-infrastructure)
  - [Sections](#sections)
  - [Section Data](#section-data)
  - [Section Names](#section-names)
- [Song Section Details](#song-section-details)
  - [Available Metadata](#available-metadata)
  - [Audio Streams](#audio-streams)
- [Track Objects](#track-objects)
- [SyncTrack Section Details](#synctrack-section-details)
  - [Time Signatures](#time-signatures)
    - [Timesig Examples](#timesig-examples)
  - [Tempos](#tempos)
    - [Tempo Examples](#tempo-examples)
  - [Tempo Anchors](#tempo-anchors)
    - [Anchor Example](#anchor-example)
- [Events Section Details](#events-section-details)
  - [Global Events](#global-events)
    - [Common Global Events](#common-global-events)
    - [Common Legacy/Unused Global Events](#common-legacyunused-global-events)
- [Instrument Section Details](#instrument-section-details)
  - [Notes & Modifiers/Flags](#notes--modifiersflags)
    - [Note/Modifier Type Divisions](#notemodifier-type-divisions)
    - [5-Fret Note Types](#5-fret-note-types)
    - [Guitar Hero Live Note Types](#guitar-hero-live-note-types)
    - [Drums Note Types](#drums-note-types)
  - [Special Phrases](#special-phrases)
    - [Special Phrase Type Divisions](#special-phrase-type-divisions)
    - [Common Special Phrase Types](#common-special-phrase-types)
    - [Drums-Specific Special Phrase Types](#drums-specific-special-phrase-types)
    - [Legacy Special Phrase Types](#legacy-special-phrase-types)
  - [Local Events](#local-events)
    - [Common Local Events](#common-local-events)
    - [Drums Local Events](#drums-local-events)
    - [Common Legacy/Unused Local Events](#common-legacyunused-local-events)
- [Documentation Notes](#documentation-notes)

## Basic Infrastructure

### Sections

Data in .chart files is grouped into sections. Sections start with their name in square brackets, and follow with their contents encapsulated by curly brackets. Section names are always written in PascalCase. Sections do not have to be separated with line breaks.

```
[SectionOne]
{
    // Section data goes here
}
[SectionTwo]
{
    // Section data goes here
}
```

### Section Data

Section data is represented by objects, each one separated by a line break. Section objects are key-value pairs:

`<Key> = <Value>`

The value type of both the key and the value depends on what section the object is contained in.

Numbers are always written in decimal.

### Section Names

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

## Using via ChartTools

WIP (Info such as reading, writing, etc. should go here)

## Song Section Details

The `Song` section contains metadata about the song and chart.

Its section objects follow this format:

`<Name> = <Value>`

`Name` is the name of the metadata entry, written in PascalCase.

`Value` is the value of the metadata entry. The data type of `Value` depends on which entry it is paired to. Strings are surrounded by quotation marks which must be removed when parsed.

The order of metadata objects may vary, so you should *not* rely on the order to identify each object, and should only rely on their keys.

This section is not used by Clone Hero for metadata (except for the chart resolution), it instead gets metadata from an accompanying song.ini file. The only time CH uses Chart metadata is if a song.ini does not exist, in which case it will create one with that metadata.

### Available Metadata

| `Key`          | Description                                                                                                | Data type |
| :------------: | :--------------------------------------------------------------------------------------------------------- | :-------: |
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

### Audio Streams

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

## Track Objects

The rest of the sections all use the same type of object for their data, which will be labelled here as track objects.

Track objects are section objects with a numerical position, and are written like this:

`<Position> = <Type Code> <Value[]>`

`Position` is a positional number and is used as the key.

`Type Code` is a string (typically a single character) which indicates what type this object is.

`Value[]` is a set of one or more individual numbers, the amount of which varies per type.

Track objects in the same section should be written in increasing order of position, and for events on the same position, they should be sorted first by type (alphabetically), then by value(s) (numerically).

Some older charts may have some objects out of positional order. Clone Hero v.23 and earlier can play songs that have out-of-order objects, but the Public Test Build marks these songs as bad songs due to issues they can cause.

## SyncTrack Section Details

The `SyncTrack` section contains tempo and time signature data for the chart.

### Time Signatures

Time signature markers use the `TS` type code, and are written like this:

`<Position> = TS <Numerator> <Denominator exponent>`

`Numerator` is the numerator to use for the time signature.

`Denominator exponent` is the power of 2 to use for the denominator of the time signature. (Optional, defaults to 2 if unspecified.) In other words: Denominator = `2 ^ <denominator exponent>`

#### Timesig Examples

4/4 is `TS 4` or `TS 4 2`.

7/16 is `TS 7 4`.

3/8 is `TS 3 3`.

### Tempos

Tempo markers use the `B` type code, and are written like this:

`<Position> = B <Tempo>`

`Tempo` is any BPM value that has up to 3 whole number places and up to 3 decimal places, multiplied by 1000. This makes valid BPM values range from 0.001 to 999.999.

#### Tempo Examples

120 BPM = `B 120000`

60 BPM = `B 60000`

150.325 BPM = `B 150325`

### Tempo Anchors

Tempo anchors use the `A` type code, and are written like this:

`<Position> = A <Audio time position>`

`Audio time position` is the position that the associated tempo marker should be set to, in microseconds.

They lock a tempo marker's position to a time position relative to the audio. The preceding tempo marker should have its tempo adjusted to maintain this lock if any of the preceding tempo markers are adjusted.

#### Anchor Example

This will anchor a 120 BPM tempo marker at position 864 to the time position at 2.25 seconds.

```
864 = A 2250000
864 = B 120000
```

## Events Section Details

The `Events` section contains global events.

### Global Events

Global events are events that appear in the Events track.

Events use the `E` type code, and are written like this:.

`<Position> = E <Event>`

`Event` is the event data.

Global event data should be surrounded by quotation marks. As a result, having quotation marks in global events is disallowed. However, some charters who want quotation marks in their lyrics might work around this by using `/"` or `"/"` if it goes before a syllable, and `"/` if it goes after. The CH PTB is able to parse quotation marks in events without these workarounds.

#### Common Global Events

These are commonly seen/read-by-CH global events:

- `phrase_start` – Start of a new lyrics phrase. Marks the end of the previous phrase if it has no end event.
- `phrase_end` – End of the current lyrics phrase. Lyrics will disappear at this event if far enough from the next start event.
- `lyric <syllable>` – Stores a syllable of the current lyric phrase as the argument. All of the lyrics within a phrase get concatenated into a single phrase in-game. Text gets highlighted one syllable at a time by the lyrics system as lyric events are passed. There are various symbols that do various things or get stripped out:
  - Hyphens `-` will combine two syllables into a single word.
  - Equals `=` will join two syllables using a hyphen.
  - Slashes `/`, underscores `_`, pluses `+`, hashes/pounds `#`, carets `^`, sections `§`, dollar signs `$`, percents `%` and anything inside triangle brackets `<>` that does not match a whitelist are either stripped out or replaced with a space by CH. Most of these are various symbols used in .mid charts for things in Rock Band, their purposes are explained in the .mid docs.
  - CH's PTB can properly parse [TextMeshPro formatting tags](http://digitalnativestudios.com/textmeshpro/docs/rich-text/), such as `<color=#00FF00>`, and has [a whitelist](https://strikeline.myjetbrains.com/youtrack/issue/CH-226) for tags that it allows. CH v.23 will break ones that include an equals sign, slash, or hash.
  - Syllables not joined together through anything will automatically be separated by a space.
- `section <name>` – Marks a new section used by Practice mode and post-game summary.
- `end` – Marks the end of a song.

#### Common Legacy/Unused Global Events

Most/all of these are from Guitar Hero 1/2 charts, and are included with Moonscraper's default list of global events.

- `idle`
- `play`
- `half_tempo`
- `normal_tempo`
- `verse`
- `chorus`
- `music_start`
- `lighting ()`
- `lighting (flare)`
- `lighting (blackout)`
- `lighting (chase)`
- `lighting (strobe)`
- `lighting (color1)`
- `lighting (color2)`
- `lighting (sweep)`
- `crowd_lighters_fast`
- `crowd_lighters_off`
- `crowd_lighters_slow`
- `crowd_half_tempo`
- `crowd_normal_tempo`
- `crowd_double_tempo`
- `band_jump`
- `sync_head_bang`
- `sync_wag`

## Instrument Section Details

### Notes & Modifiers/Flags

Notes and modifiers use the `N` type code and are written like this:

`<Position> = N <Type> <Length>`

`Type` is the type of note/modifier that this object represents.

`Length` is the length of a note. This value doesn't do anything for modifiers.

As with tempo anchors, modifiers are applied to notes but are listed as separate objects. A modifier applies to all notes in the same position, so different notes of the same position cannot have different modifiers unless the modifier specifically targets a single color/type.

HOPOs get forced automatically based on the distance to the previous note. The default threshold is 65/192 ticks in a 192 resolution, and this ratio gets recalculated for other resolutions.

#### Note/Modifier Type Divisions

To help prevent issues such as Black 3 in the GHL notes having a non-sequential value of 8, moving forward the types will be divided into groups of 32:

- 0 through 31 for standard GH notes
- 32 through 63 for GH-specific notes/modifiers such as Expert+ kicks
- 64 through 95 for Rock Band-specific notes/modifiers such as cymbal markers
- 96 through 127 for CH-specific notes/modifiers

Any games that want to add custom note/modifier types should reserve the next set of 32.

#### 5-Fret Note Types

- 0 – Green note
- 1 – Red note
- 2 – Yellow note
- 3 – Blue note
- 4 – Orange note
- 5 – Strum/HOPO flip (force) modifier
- 6 – Tap modifier
- 7 – Open note

#### Guitar Hero Live Note Types

- 0 – White 1 note
- 1 – White 2 note
- 2 – White 3 note
- 3 – Black 1 note
- 4 – Black 2 note
- 5 – Strum/HOPO flip (force) modifier
- 6 – Tap modifier
- 7 – Open note
- 8 – Black 3 note

#### Drums Note Types

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

### Special Phrases

Special phrase objects use the `S` type code and are written like this:

`<Position> = S <Type> <Length>`

These are other track-specific objects that do not fit under the description of notes/modifiers.

#### Special Phrase Type Divisions

The same type divisions of 32 as for notes/modifiers should be applied here:

- 0 through 31 for standard specials
- 32 through 63 for GH-specific specials
- 64 through 95 for Rock Band-specific specials
- 96 through 127 for CH-specific specials

#### Common Special Phrase Types

- 2 - Star power

#### Drums-Specific Special Phrase Types

- 64 - Drums SP activation phrase

#### Legacy Special Phrase Types

- 0 - GH1/2 Co-op primary player (Legacy)
- 1 - GH1/2 Co-op secondary player (Legacy)

### Local Events

Local events are events that appear in instrument tracks.

Events use the `E` type code, and are written like this:

`<Position> = E <Event>`

`Event` is the event data.

Local events are written plainly with no surrounding symbols, and cannot have spaces in them. There may be .chart files out there with spaces in local events, though.

#### Common Local Events

These are local events available on all instruments.

- `solo` - Starts a solo.
- `soloend` - Ends a solo.

#### Drums Local Events

- `mix_<difficulty>_drums0d` - Enables disco flip on drums. For `difficulty`, 3 is Expert, 0 is Easy, etc.
- `mix_<difficulty>_drums0` - Disables disco flip on drums. `difficulty` is the same as above.
  - (Moonscraper includes these events as `[mix <difficulty> drums0d]` and `[mix <difficulty> drums0]` since they're lifted straight from .mid, but since spaces aren't allowed in local events, this will force users to save in .msce and export the chart for it to be playable. The events as written in these docs is how Moonscraper exports the .mid-lifted ones to .chart, and how CH reads them from .chart.)

#### Common Legacy/Unused Local Events

Most of these are from GH1/2, and are included with Moonscraper's default local events list (excluding `solo_on` and `solo_off`).

- `ghl_6` - Indicates that a note should be a GHL 6th fret note. Only has an effect in Moonscraper when exporting to .mid, pretty sure.
- `ghl_6_forced` - Indicates that a note should be a GHL forced 6th fret note. Only has an effect in Moonscraper when exporting to .mid, pretty sure.
- `solo_on` - Solo start event for GH1/2.
- `solo_off` - Solo end event for GH1/2.
- `wail_on`
- `wail_off`
- `ow_face_on`
- `ow_face_off`

## Documentation Notes

A large part of this info comes from [Firefox's .chart specifications](https://docs.google.com/document/d/1v2v0U-9HQ5qHeccpExDOLJ5CMPZZ3QytPmAG5WF0Kzs/edit#).
