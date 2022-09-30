# Getting Started
This document goes over the basics of parsing a chart through ChartTools.

## Installation
To add ChartTools to your project, you must first compile the assembly. The repository contains two main projects: Net5 and Net6. Compile the project that matches your target .NET version with either the Debug or Release profile. The compiled files can be found in the build folder of the repository.

Visual Studio: Right-click on your from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.dll that was generated.

## Sandbox environment
The repository includes a sandbox environment where you can explore the library and perform various tests. To use the sandbox environment, open Program.cs from the Debug project. The project targets .NET 6 and references the Net6 project. After cloning the repo, open Git Bash and navigate to the Debug directory, then run the command `git ls-files -z | xargs -0 git update-index --assume-unchanged` to ensure changes are not included in commits. This command only needs to be ran once.

> **NOTE**: Pull requests with leftover tests will be rejected.

## Supported file formats
ChartTools supports parsing of .chart and .ini files, with .mid parsing in the works. [mdsitton's BChart](https://github.com/mdsitton/bchart) format is planned as a post-launch update.

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

When reading from a directory, the metadata will be read from `song.ini`, followed by the rest read from `notes.chart`.

A song contains four main components:

- Metadata - Miscellaneous info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- [Global events](Events.md) - Events that are not tied to an instrument.
- Instruments - The instrument track data.

Each of these components can be read individually from a file or directory using the non-generic version of the corresponding class, with or without a configuration object.

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

When reading from multiple files, you can mix file types and priority of information is defined by the order of the files.

As a future-proofing method, all unsupported items can be found under UnidentifiedData. This data will only be written to the same file format as the one it was read from.

### Instruments and Tracks
All instruments currently supported are represented using the generic `Instrument` class. This class contains an object of type `Track` class for every difficulty. A track can be retrieved from a song with the following code.

```csharp
Track<StandardChord> track = song.Instruments.LeadGuitar.Expert;
```

Notice the use of StandardChord as a generic type. Instruments are divided into four categories based on the type of chords it uses. These categories are

- Standard - Five colored notes
- Drums - Five colored with support for double kick and cymbal flags
- GHL (Guitar Hero Live) - Three black and three white notes
- Vocals - Notes with an associated syllable. Only one note can be added for the same position. [Learn more about vocals](Lyrics.md).

A track is composed of three components:

- Chords (defined by the generic type)
- Special phrases (star power)
- [Local events](Events.md)

> **NOTE**: When setting an instrument in an `InstrumentSet` or a track in an instrument, a copy of the object is created which contains information about its indentity. In order to have changes made to the object after the assignement be reflected, a reference must be re-obtained from the parent.

Instruments can also be obtained dynamically from a song regardless of the type. [Learn more about the dynamic syntax](DynamicSyntax.md).

### Chords and Notes
A chord is a set of notes played at the same time. All supported instruments use the generic version of the Chord class where the generic type defines the type of notes contained. The note types are the same as the types of instruments as listed in the section. The types for notes are

- `Note<StandardLane>`
- `Note<GHLLane>`
- `DrumsNote`
- `Syllable` (Vocals)

The following adds an orange note to every chord on a track.

```csharp
foreach (StandardChord chord in song.Instruments.LeadGuitar.Expert)
{
    chord.Notes.Add(StandardLane.Orange);
    // or
    chord.Notes.Add(new Note<StandardLane>(StandardLane.Orange));
}
```

## Optimizing
Although still functional, some files may contain data that slow down the reading process or in worse cases, may result in non-functional files when saved in certain formats. ChartTools provides various utilities to fix such issues.

```csharp
using ChartTools.Optimization;

chords.CutSustains() // Cuts short sustains that exceed the position of the next identical note
chords.CutLengths() // Cuts short star power phrases that exceed the start of the next phrase

// Sorts and removes redundant markers
syncTrack.Tempo.RemoveUnneeded();
syncTrack.TimeSignatures.RemoveUnneeded();
```

ChartTools includes other utilities for various purposes. [Learn more](Tools.md).

## Writing files
Finally, changes can be saved to a file using the instance or extension `ToFile` method of most components. The format is determined based on the extension of the file. For instruments and tracks, the existing component will be overwritten or added while keeping the rest of the file if it already exists. [Learn more about configuring IO operations](Configuration.md).

```csharp
song.ToFile(path, <WritingConfiguration>); // .chart file only
metadata.ToFile(path); // .chart or .ini - Some properties may not be written depending on the output format
```

When writing an individual component, it is recommended to pass the formatting to avoid it being read incorrectly in the future.

```csharp
instrument.ToFile(path, <WritingConfiguration>, metadata.Formatting);
```
