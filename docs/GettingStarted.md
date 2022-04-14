# Getting Started

This document goes over the basics of parsing a chart through ChartTools. The full documentation can be found [here]().

## Supported File Formats

ChartTools supports parsing of .chart and .ini files, with .mid parsing in the works.

For documentation on the formats themselves, refer to the [GuitarGame_ChartFormats](https://github.com/TheNathannator/GuitarGame_ChartFormats) repository.

## Song

Every component of a chart is stored in an instance of the Song class. It can be initialized by reading a file that will be parsed based on the extension.

```c#
Song song = Song.FromFile(path);
```

A configuration object may also be used to customize the error handling behavior:

```c#
Song song = Song.FromFile(path, new ReadingConfiguration());
```

Note: Some MIDI files may contain formatting information in the song.ini file. In order to account for custom formatting when reading, it is recommended to read from a directory instead:

```c#
Song song = Song.FromDirectory(path, <ReadingConfiguration>);
```

When reading from a directory, the metadata will be read from `song.ini`, followed by the rest read from `notes.chart`.

A song contains four main components:

- Metadata - Miscellaneous info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- Global events - Events that are not tied to an instrument.
- Instruments - The instrument track data.

Each of these components can be read individually from a file or directory using the non-generic version of the corresponding class, with or without a configuration object.

## Metadata

Similar to reading a song, metadata is retrieved by reading a file:

```c#
Metadata metadata = Metadata.FromFile(path);
```

Metadata can be read from either .chart or .ini. Given that most modern charts are made for Clone Hero, it is recommended that you prioritize .ini over .chart metadata, as that will usually be the more accurate metadata.

Metadata can also be retrieved from multiple files at once:

```c#
Metadata metadata = Metadata.FromFiles(path1, path2, path3...);
```

When reading from multiple files, you can mix file types and priority of information is defined by the order of the files.

As a future-proofing method, all unsupported items can be found under UnidentifiedData. This data will only be written to the same file format as the one it was read from.

## Instruments and Tracks

All instruments currently supported are represented using the generic `Instrument` class. This class contains an object of type `Track` class for every difficulty. A track can be retrieved from a song like this:

```c#
Track<StandardChord> track = song.Instruments.LeadGuitar.Expert;
```

Notice the use of StandardChord as a generic type. Instruments are divided into four categories based on the type of chords it uses. These categories are:

- Standard - Five colored notes
- Drums - Five colored with support for double kick and cymbal flags
- GHL (Guitar Hero Live) - Three black and three white notes
- Vocals - Notes with an associated syllable. Only one note can be added for the same position.

A track is composed of three components:

- Chords (defined by the generic type)
- Special phrases (star power)
- Local events

> **NOTE**: When setting a track in an instrument, a clone of the track is created which contains the target instrument as the `ParentInstrument` property. In order to have changes made to the track after the assignment be reflected in the instrument, the track must be re-obtained from the instrument rather than using the instance used in the assignment.

You can also get instruments from a song dynamically regardless of the type. To learn more, check out [Dynamic Syntax](DynamicSyntax.md).

## Chords and Notes

A chord is a set of notes played at the same time. All supported instruments use the generic version of the Chord class where the generic type defines the type of notes contained. The note types are the same as the types of instruments as listed in the section. The types for notes are:

- Note\<StandardLane\>
- Note\<GHLLane\>
- DrumsNote
- Syllable (Vocals)

The following adds an orange note to every chord on a track:

```c#
foreach (StandardChord chord in song.Instruments.LeadGuitar.Expert)
{
    chord.Notes.Add(StandardLane.Orange);
    // or
    chord.Notes.Add(new Note<StandardLane>(StandardLane.Orange));
}
```

## Lyrics

Lyrics of a song are defined by a collection of phrases. A phrase represents a single line of lyrics, which are split up into syllables.

Phrases can either be read from a file:

```c#
using ChartTools.Lyrics;

IEnumerable<Phrase> lyrics = Phrase.FromFile(path);
```

or using existing global events:

```c#
using ChartTools.Lyrics;

IEnumerable<Phrase> lyrics = song.GlobalEvents.GetLyrics();
```

To be written to a file, lyrics must be converted back into global events:

```c#
using ChartTools.Lyrics;

lyrics.ToGlobalEvents(); // Creates a new set of global events
events.SetLyrics(lyrics); // Replaces existing lyric-related events with new events making up the phrases
```

It is also possible to edit lyrics directly from the global events. The use of phrase and syllable objects are intended to simplify the editing of lyrics, and any changes to these objects are only applied to the song once they are converted back into global events.

## Optimizing

Although still functional, some files may contain data that slow down the reading process or in worse cases, may result in non-functional files when saved in certain formats. ChartTools provides various utilities to fix such issues:

```c#
using ChartTools.Optimization;

chords.CutSustains() // Cuts short sustains that exceed the position of the next identical note
chords.CutLengths() // Cuts short star power phrases that exceed the start of the next phrase

// Sorts and removes redundant markers
syncTrack.Tempo.RemoveUnneeded();
syncTrack.TimeSignatures.RemoveUnneeded();
```

## Writing files

Finally, changes can be saved to a file using the instance or extension `ToFile` method of most components. The format is determined based on the extension of the file. For instruments and tracks, the existing component will be overwritten or added while keeping the rest of the file if it already exists.

```c#
song.ToFile(path, <WritingConfiguration>); // .chart file only
metadata.ToFile(path); // .chart or .ini - Some properties may not be written depending on the output format
```

When writing an individual component, it is recommended to pass the formatting to avoid it being read incorrectly in the future.

```c#
instrument.ToFile(path, <WritingConfiguration>, metadata.Formatting);
```
