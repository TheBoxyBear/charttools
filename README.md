# ChartTools
ChartTools is a C# library with the purpose of modeling song files for the video game Clone Hero. It currently supports reading of chart and ini files, with MIDI support currently in development.

NOTE: In order to future-proof the library for features added to Clone Hero, improve code readability and insure long-term support, future development will be made in .NET 6. While .NET 6 is in preview, both .NET versions will be supported, with new features being added to the net6-beta branch before being brought to net5-stable. Once the preview period is over, the stable branch will be brought to .NET 6.

Special thanks to [FireFox](https://github.com/FireFox2000000) for making the Moonscraper editor open-source and to members of the [Clone Hero Discord](https://discord.gg/clonehero) and [Moonscraper Discord](https://discord.gg/wdnD83APhE) including but not limited to DarkAngel2096, drumbs, FireFox, Kanske, mdsitton, Spachi and XEntombmentX for their help in researching.

This library is an independent project not affiliated with Clone Hero or Moonscraper.

## Installation
To add ChartTools to your project using Visual Studio, right-click on Dependencies under your project from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.dll from the repository. All classes are located under the ChartTools namespace. This version runs on .NET 5.

If you find any bugs, you can report them in the [Issues section](https://github.com/TheBoxyBear/ChartTools/issues) of the repository. Make sure to use the "bug" label.

## Getting started
Every element of a chart is stored in an instance of the Song class. It can be initialized by reading a file. The file format is detected automatically using the extension:
```c#
Song song = Song.FromFile(filePath);
```
A configuration object may also be used to customize the error handling behavior:
```c#
Song song = Song.FromFile(filePath, new ReadingConfiguration {});
```

A song contains five main elements:

- Metadata - Miscellaneous info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- Global events
- Instruments

Each of these elements can be read individually from a file using the non-generic version of the corresponding class, with or without a configuration object.

## Metadata
Similar to reading a song, metadata can be read from one or multiple files:
```c#
Metadata metadata = Metadata.FromFile(filePath);
Metadata metadata = Metada.FromFiles(filePath1, filePath2, filePath3...);
```
Files can be .chart or .ini. When reading from multiple files and the extensions can be mixed together. The first file has utmost priority and other files are only read to retrieve the missing information, in decreasing order of priority until all properties of the metadata have a value or until the last file is read.

As a future-proofing method, all unsupported items can be found under UnidentifiedData. This data will only be written to the same file format as the one it was read from.

## Instruments and tracks
All instrument currently supported are represented using the generic Instrument class. This class contains a track for every difficulty level. A track can be retrieved from a song like so:
```c#
Track<StandardChord> track = song.LeadGuitar.Expert;
```
Notice the use of StandardChord used as a generic type. Instruments are divided in three categories based on the type of chords it uses. These categories are:
- Standard - Five colored notes
- Drums - Five colored with support for double kick and cymbal flags
- GHL (Guitar Hero Live) - Three black and three white notes

A track is composed of three elements:
- Chords (defined by the generic type)
- Star power phrases
- Local events

## Chords and notes
A chord is a set of notes played at the same time. All supported instruments use the generic version of the Chord class where the generic type defines the type of notes contained. The note types are the same as the types of instruments as listed in the section. The types for notes are:

- Note\<StandardLane>
- Note\<GHLLane>
- DrumsNote

Drums is an exception case where a class is specifically defined as it contains exclusive properties. It inherits from Note\<DrumsFret>.

The following adds an orange note to every chord on a track:
```c#
foreach (StandardChord chord in song.LeadGuitar.Expert)
{
    chord.Notes.Add(StandardLane.Orange);
    // or
    chord.Notes.Add(new Note<StandardLane>(StandardLane.Orange));
}
```

## Lyrics
Lyrics of a song are defined by a collection of phrases. A phrase represents a single line displayed on-screen. It contains syllables, each one representing a section of the text which changes color by the karaoke system. Phrases can either be read from a file:
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

It is also possible to edit lyrics directly from the global events. The use of phrase and syllable objects are intended to simplify the editing of lyrics and any changes to these objects are only applied to the song once they are converted back into global events.

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
Finally, changes can be saved to a file using the instance or extension ToFile method of most components. The format is based on the extension of the file. For instruments and tracks, the existing element will be overwritten or added while keeping the rest of the file if it already exists.
```c#
song.ToFile(filePath); // .chart file only
metadata.ToFile(filePath) // .chart or .ini - some properties may not be written depending on the output format
song.GlobalEvents.ToFile(path) // .chart only
```
Due to strict limitations with the MIDI format, it is recommended to use a custom configuration object when writing files.
