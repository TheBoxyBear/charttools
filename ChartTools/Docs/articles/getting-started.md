# Getting Started
This document will go over the basics of parsing and saving a chart using ChartTools.

## Installation
To add ChartTools to your project, you must first build the ChartTools project. This will generate the dll under `ChartTools/bin/[Debug][Release]`.

Visual Studio: Right-click on your project from the solution explorer and select "Add Project References...". Click on "Browse" and select `ChartTools.dll` that was generated. If the dll file is moved, also move `ChartTools.xml` to the new directory for XML documentation.

## Supported file formats
ChartTools supports the parsing of .chart and .ini files, with .mid parsing in the works. Unless written as `.chart`, the term "chart" refers to songs supported by ChartTools regardless of the file format.

For documentation on the formats themselves, refer to the [GuitarGame_ChartFormats](https://github.com/TheNathannator/GuitarGame_ChartFormats) repository by TheNathannator.

## Working with charts

### Song
Every component of a chart is stored in an instance of the [Song](~/api/ChartTools.Song.yml) class. It can be initialized by reading a file that will be parsed based on the extension.

```csharp
Song song = Song.FromFile(path);
```

A configuration object may also be used to customize the error handling behavior. [Learn more about configuring IO operations](~/articles/configuration.md).

```csharp
Song song = Song.FromFile(path, new ReadingConfiguration());
```

Most songs are composed of a primary file, typically `notes.chart`, and a metadata file, typically `song.ini`. Both files can be read and their data combined by reading from a directory.

When reading from a directory, the metadata will be read from `song.ini`, followed by the rest from `notes.chart`.

```csharp
Song song = Song.FromDirectory(path, <ReadingConfiguration>);
```

> [!IMPORTANT]
> Some file formats rely on formatting information in the `song.ini` file. In order to account for custom formatting when reading, it is recommended to read from a directory instead.

### Components

A song contains four main components:

- [Metadata](~/api/ChartTools.Metadata.yml) - Miscellaneous info about the song, such as title, album, charter etc.
- [Sync track](~/api/ChartTools.SyncTrack.yml) - Markers that define time signature and tempo
- [Global events](~/articles/events.md) - Events not tied to an instrument or track
- [Instruments](~/api/ChartTools.InstrumentSet.yml) - Instruments, tracks and notes
- [Vocals](~/api/ChartTools.Lyrics.Vocals.yml) - Vocals notes paired with lyric text

> [!NOTE]
> Although vocals are typically considered as an instrument, their rules and data representations are inheritly different than other instruments. Therefore, ChartTools considers vocals as a distinct component.


### Metadata
Similar to reading a song, [Metadata](~/api/ChartTools.Metadata.yml) can be read from a file:

```csharp
Metadata metadata = Metadata.FromFile(path);
```

Metadata can be read from either .chart or .ini. Given that most modern charts are made for Clone Hero, it is recommended that you prioritize .ini over .chart metadata, as that will usually be the more accurate metadata.

Metadata can also be retrieved from multiple files at once.

```csharp
Metadata metadata = Metadata.FromFiles(path1, path2, path3, ...);
```

When reading from multiple files, you can mix file types, and the priority of information is defined by the order of the paths in the call.

As a future-proofing method, any unrecognized metadata is stored in an internal dictionary of strings. This dictionary can be accessed through the `Get`, `Set`, and `Remove` methods. When trying to use one of these methods with a recognize metadata key, the value instead be parsed and serialized on the fly from the matching property. Although this mapping of properties is performed using attributes, these attributes are resolved and converted to static code built into the library. This ensures a minimal performance cost of using the dynamic dictionary.

### Instruments and Tracks
An instrument is a collection of tracks, each representing a difficulty level. ChartTools currently supports the following instruments:

- Lead guitar
- Rhythm guitar
- Co-op guitar
- Bass
- Drums
- Keys
- Guitar Hero Live lead guitar
- Guitar Hero Live rhythm guitar
- Guitar Hero Live co-op guitar
- Guitar Hero Live bass

> [!NOTE]
> ChartTools also supports vocals, but does not consider them as an instrument. [Learn more about vocals](~/articles/vocals.md).

A track can be retrieved from a song as such:

```csharp
Track<StandardChord> track = song.Instruments.StandardLeadGuitar.Expert;
```

Notice the use of [StandardChord](~/api/ChartTools.StandardChord) as a generic type. Instruments are divided into four categories based on the type of chords they use. These categories are:

- Standard - Five colored notes
- Drums - Five colored notes with support for double kick and cymbal flags
- GHL (Guitar Hero Live) - Three black and three white notes

A track is composed of three components:

- Chords - Defined by the generic type
- Special phrases - Primarily used for star power
- [Local events](~/articles/events.md)

Instruments can also be obtained dynamically from a song, regardless of the type. [Learn more about the dynamic syntax](~/articles/dynamic-syntax.md).

### Chords and Notes
A chord is a set of notes played at the same time. For readability, most chords and notes have specific classes for each instrument type, deriving from [Chord<TNote, TLane, TModifiers>](~/api/ChartTools.Chord-3) and [LaneNode\<TLane\>](~/api/ChartTools.LaneNote-1).

The following snippet adds an orange note to every chord on a track:

```csharp
foreach (StandardChord chord in song.Instruments.StandardLeadGuitar.Expert)
{
    chord.Notes.Add(StandardLane.Orange);
    // or
    chord.Notes.Add(new LaneNote<StandardLane>(StandardLane.Orange));
}
```

## Tools
As the name suggests, *ChartTools* also provides various tools to help fix and optimize songs. [Learn more](~/articles/tools.md).

## Writing files
Finally, changes can be saved to a file, with the format determined by the file extension.

```csharp
song.ToFile("output.chart", <WritingConfiguration>);
```

Don't forget to also write the metadata to `song.ini`.

```csharp
song.Metadata.ToFile("song.ini", <WritingConfiguration>);
```