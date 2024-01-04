# Getting Started
This document goes over the basics of parsing a chart through ChartTools.

## Installation
To add ChartTools to your project, you must first compile the assembly. The repository contains two main projects: Net6 and Net7. Compile the project that matches your target .NET version with either the Debug or Release profile. The compiled files can be found in the build folder of the repository.

Visual Studio: Right-click on your project from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.dll that was generated.

## Supported file formats
ChartTools supports the parsing of .chart and .ini files, with .mid parsing in the works. [mdsitton's BChart](https://github.com/mdsitton/bchart) format is planned as a post-launch update. Unless written as `.chart`, the term "chart" refers to songs supported by ChartTools regardless of the file format.

For documentation on the formats themselves, refer to the [GuitarGame_ChartFormats](https://github.com/TheNathannator/GuitarGame_ChartFormats) repository.

## Working with charts

### Song
Every component of a chart is stored in an instance of the Song class. It can be initialized by reading a file that will be parsed based on the extension.

```csharp
Song song = Song.FromFile(path);
```

A configuration object may also be used to customize the error handling behavior. [Learn more about configuring IO operations](Configuration.md).

```csharp
Song song = Song.FromFile(path, new ReadingConfiguration());
```

Note: Some MIDI files may contain formatting information in the song.ini file. In order to account for custom formatting when reading, it is recommended to read from a directory instead.

```csharp
Song song = Song.FromDirectory(path, <ReadingConfiguration>);
```

When reading from a directory, the metadata will be read from `song.ini`, followed by the rest from `notes.chart`.

A song contains four main components:

- Metadata - Miscellaneous info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- [Global events](Events.md) - Events that are not tied to an instrument
- Instruments - The instrument track data

### Metadata
Similar to reading a song, metadata is retrieved by reading a file:

```csharp
Metadata metadata = Metadata.FromFile(path);
```

Metadata can be read from either .chart or .ini. Given that most modern charts are made for Clone Hero, it is recommended that you prioritize .ini over .chart metadata, as that will usually be the more accurate metadata.

Metadata can also be retrieved from multiple files at once.

```csharp
Metadata metadata = Metadata.FromFiles(path1, path2, path3...);
```

When reading from multiple files, you can mix file types, and the priority of information is defined by the order of the files.

As a future-proofing method, all unsupported items can be found under UnidentifiedData. This data will only be written to the same file format as the one it was read from.

### Instruments and Tracks
All instruments currently supported are represented using the generic `Instrument` class. This class contains an object of type `Track` class for every difficulty. A track can be retrieved from a song with the following code:

```csharp
Track<StandardChord> track = song.Instruments.LeadGuitar.Expert;
```

Notice the use of StandardChord as a generic type. Instruments are divided into four categories based on the type of chords they use. These categories are:

- Standard - Five colored notes
- Drums - Five colored with support for double kick and cymbal flags
- GHL (Guitar Hero Live) - Three black and three white notes
- Vocals - Notes with an associated syllable. Notes have positions defined as offsets from the phrase. [Learn more about vocals](Lyrics.md).

A track is composed of three components:

- Chords (defined by the generic type)
- Special phrases (star power)
- [Local events](Events.md)

> **NOTE**: When setting an instrument in an `InstrumentSet` or a track in an instrument, a copy of the object is created that contains information about its identity. In order to have changes made to the object after the assignment be reflected, a reference must be re-obtained from the parent.

Instruments can also be obtained dynamically from a song, regardless of the type. [Learn more about the dynamic syntax](DynamicSyntax.md).

### Chords and Notes
A chord is a set of notes played at the same time. All supported instruments use the generic version of the Chord class, where the generic type defines the type of notes contained. The note types are the same as the types of instruments listed in the section. The types of notes are

- `Note<StandardLane>`
- `Note<GHLLane>`
- `DrumsNote`
- `Syllable` (Vocals)

The following adds an orange note to every chord on a track:

```csharp
foreach (StandardChord chord in song.Instruments.LeadGuitar.Expert)
{
    chord.Notes.Add(StandardLane.Orange);
    // or
    chord.Notes.Add(new Note<StandardLane>(StandardLane.Orange));
}
```

## Optimizing
Although still functional, some files may contain data that slows down the reading process or, in worse cases, may result in non-functional files when saved in certain formats. ChartTools provides various utilities to fix such issues.

```csharp
using ChartTools.Optimization;

chords.CutSustains() // Cuts short sustains go past what is applicable in-game.
specialPhrases.CutSpecialLenghts.CutLengths() // Cuts short special phrases that exceed the start of the next phrase based on type.

// Sorts and removes redundant markers
syncTrack.Tempo.RemoveUnneeded();
syncTrack.TimeSignatures.RemoveUnneeded();
```

ChartTools includes other utilities for various purposes. [Learn more](Tools.md).

## Writing files
Finally, changes can be saved to a file using the `ToFile` method of the `Song` class, with the format determined by the file extension.

```csharp
song.ToFile("output.chart", <WritingConfiguration>);
```

Due to limitations of certain file formats, only `Song` objects can be written to a file in this manner. Format-specific operations can be accessed through the respective static class, such as `ChartFile` for `.chart`. For example, here is how to replace an instrument in a `.chart` file.

```csharp
ChartFile.ReplaceInstrument("output.chart", guitar, <WritingConfiguration>);
```


Like when reading files, writing operations can be configured to alter how they deal with errors. [Learn more about configuring IO operations](Configuration.md).