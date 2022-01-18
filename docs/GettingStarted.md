# Getting Started

This document goes over the basics of parsing a chart through ChartTools.

## Supported File Formats

ChartTools supports parsing of .chart and .ini files, with .mid parsing in the works.

For documentation on the formats themselves, refer to the [GuitarGame_ChartFormats](https://github.com/TheNathannator/GuitarGame_ChartFormats) repository.

## Song

Every element of a chart is stored in an instance of the Song class. It can be initialized by reading a file. The file format is detected automatically using the extension:

```c#
Song song = Song.FromFile(filePath);
```

A configuration object may also be used to customize the error handling behavior:

```c#
Song song = Song.FromFile(filePath, new ReadingConfiguration {});
```

A song contains four main elements:

- Metadata - Miscellaneous info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- Global events - Events that are not tied to an instrument.
- Instruments - The instrument track data.

Each of these elements can be read individually from a file using the non-generic version of the corresponding class, with or without a configuration object.

## Metadata

Similar to reading a song, metadata is retrieved by reading a file:

```c#
Metadata metadata = Metadata.FromFile(filePath);
```

Metadata can be read from either .chart or .ini. Given that most modern charts are made for Clone Hero, it is recommended that you prioritize .ini over .chart metadata, as that will usually be the more accurate metadata.

Metadata can also be retrieved from multiple files at once:

```c#
Metadata metadata = Metadata.FromFiles(filePath1, filePath2, filePath3...);
```

When reading from multiple files, you can read from multiple file types at the same time, and the first file's information is prioritized: metadata from following files files is only read to fill out missing information, in decreasing order of priority, until either all metadata properties have a value, or until the last file is read. Similarly to reading a single file, it is recommended to read any .ini files first.

As a future-proofing method, all unsupported items can be found under UnidentifiedData. This data will only be written to the same file format as the one it was read from.

## Instruments and Tracks

All instruments currently supported are represented using the generic `Instrument` class. This class contains a `Track` class for every difficulty. A `Track` can be retrieved from a song like this:

```c#
Track<StandardChord> track = song.LeadGuitar.Expert;
```

Notice the use of StandardChord as a generic type. Instruments are divided into four categories based on the type of chords it uses. These categories are:

- Standard - Five colored notes
- Drums - Five colored with support for double kick and cymbal flags
- GHL (Guitar Hero Live) - Three black and three white notes
- Vocals - Notes with an associated syllable. Only one note can be added for the same position.

A track is composed of three elements:

- Chords (defined by the generic type)
- Star power phrases
- Local events

## Chords and Notes

A chord is a set of notes played at the same time. All supported instruments use the generic version of the Chord class where the generic type defines the type of notes contained. The note types are the same as the types of instruments as listed in the section. The types for notes are:

- Note\<StandardLane\>
- Note\<GHLLane\>
- DrumsNote
- Syllable (Vocals)

Drums is an exception case where a class is specifically defined as it contains exclusive properties. It inherits from Note\<DrumsLane>.

The following adds an orange note to every chord on a track:

```c#
foreach (StandardChord chord in song.LeadGuitar.Expert)
{
    chord.Notes.Add(StandardLane.Orange);
    // or
    chord.Notes.Add(new Note<StandardLane>(StandardLane.Orange));
}
```

### Loose Syntax

ChartTools supports a loose syntax to retrieve instruments and tracks using enums instead of explicit properties.

```C#
Instrument guitar = song.GetInstrument(InstrumentIdentity.LeadGuitar);
Instrument<StandardChord> bass = song.GetInstrument(StandardInstrumentIdentity.Bass);

Track easyGuitar = guitar.GetTrack(Difficulty.Easy);
Track<StandardChord> easyBass = bass.GetTrack(Difficulty.Easy);
```

Unless the instrument type is defined through its respective enum, getting an instrument through the loose syntax returns an instance of the base Instrument class. Like with getting instruments, you can get tracks through either the loose or strong syntax. With both approaches, the base instrument class returns base tracks while explicit instruments return explicit tracks. Base tracks grant full access to local events and star power. Some instrument like drums cannot be retrieved using the loose syntax to get a generic instrument as they are the only oe in their category. For those instruments, use the explicit property.

You can get chords from a base track, in which case the chords will be of the base chord class and the collection will be read-only. In the same way, getting notes from a base chord returns a read-only, non-indexable set of base notes which grant access to their index (the numerical value in the lane enum for the respective note type) and the sustain length.

## Lyrics

Lyrics of a song are defined by a collection of phrases. A phrase represents a single line of lyrics, which are split up into syllables.

Phrases can either be read from a file:

```c#
using ChartTools.Lyrics;

IEnumerable<Phrase> lyrics = Phrase.FromFile(filePath);
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

Finally, changes can be saved to a file using the instance or extension `ToFile` method of most components. The format is determined based on the extension of the file. For instruments and tracks, the existing element will be overwritten or added while keeping the rest of the file if it already exists.

```c#
song.ToFile(filePath); // .chart file only
metadata.ToFile(filePath) // .chart or .ini - some properties may not be written depending on the output format
song.GlobalEvents.ToFile(path) // .chart only
```

Due to strict limitations with the MIDI format, it is recommended to use a custom configuration object when writing files.
