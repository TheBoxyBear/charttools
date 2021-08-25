# ChartTools
ChartTools is a C# library using .NET 5 with the purpose of modeling song files for the video game Clone Hero. It currently supports reading of chart and ini files, with MIDI support currently in development.

Special thanks to [FireFox](https://github.com/FireFox2000000) for making the Moonscraper editor open-source and to members of the [Moonscraper Discord](https://discord.gg/wdnD83APhE) including but not limited to DarkAngel2096, FireFox, Kanske, mdsitton, Spachi and XEntombmentX for their help in researching.

This library is an independent project not affiliated with Clone Hero or Moonscraper.

## Installation
To add ChartTools to your project using Visual Studio, right-click on Dependencies under your project from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.dll from the repository. All classes are located under the ChartTools namespace. 

The DLL contains the latest stable build. To get the latest version, clone tis repository and compile the project or add the project to your solution and add it as a dependency through a project reference. This version may contain more bugs than the pre-built DLL.

If you find any bugs, you can report them in the [Issues section](https://github.com/TheBoxyBear/ChartTools/issues) of the repository.

## Getting started
Every element of a chart is stored in an instance of the Song class. It can be initialized by reading a file. The file format is detected automatically using the extension:
```c#
Song song = Song.FromFile(filePath);
```
A song contains five main elements:

- Metadata - Miscellaneous info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- Global events
- Instruments

Each of these elements can be read individually from a file using the non-generic version of the corresponding class.

## Metadata
Similar to reading a song, metadata can be read from one or multiple files:
```c#
Metadata metadata = Metadata.FromFile(filePath);
Metadata metadata = Metada.FromFiles(filePath1, filePath2, filePath3...);
```
Files can be .chart or .ini. When reading from multiple files, the provided files can be a mix of different formats, each read based on its extension. The first file has utmost priority and other files are only read to retrieve the missing information, in decreasing order of priority until all properties of the metadata have a value or until the last file is read.

As a future-proofing method, all unsupported items can be found under UnidentifiedData. This data will onlty be written to the same file format as the one it was read from.

## Instruments and tracks
All instrument currently supported are represented using the generic Instrument class. Notes and other information are stored in a track, with each difficulty level being its own track. Each generic instrument contains one track per difficulty setting. A track can be retrieved from a song like so:
```c#
Track<StandardChord> track = song.LeadGuitar.Expert;
```
Notice the use of StandardChord used as a generic type. Instruments are divided in three categories based on the type of chords it uses. These categories are:
- Standard - Five colored notes
- Drums - Five colored notes where open notes (kick) can be mixed with regular notes
- GHL (Guitar Hero Live) - Three black and three white notes

A track is composed of three elements:
- Chords (defined by the generic type)
- Star power phrases
- Local events

## Chords and notes
A chord is a set of notes played at the same time. All supported instruments use the generic version of the Chord class where the generic type defines the type of notes contained. The note types are the same as the types of instruments as listed above in the previous section. Each note in a chord must be a different fret/pad and each chord in a track must have a unique position. Trying to add a duplicate note or chord will overwrite the existing one. The following adds an orange note to every chord on a track:
```c#
foreach (StandardChord chord in track)
{
    chord.Notes.Add(StandardNotes.Orange);
    // or
    chord.Notes.Add(new StandardNote(StandardNotes.Orange));
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
syncTrack.Tempo.RemoveUneeded();
syncTrack.TimeSignatures.RemoveUneeded();
```
## Writing files
Finally, changes can be saved to a file using the instance or extension ToFile method of most components. The format is based on the extension of the file. For instruments and tracks, the existing element will be overwritten or added while keeping the rest of the file if it already exists.
```c#
song.ToFile(filePath); // .chart file only
metadata.ToFile(filePath) // .chart or .ini - some properties may not be written depending on the output format
song.GlobalEvents.ToFile(path) // .chart only
```
Due to strict limitations with the MIDI format, it is recommended to use the ToFile overloads which make use of a WritingConfiguration object for more control over the conversions needed to write to MIDI.
