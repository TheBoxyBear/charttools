# ChartTools
ChartTools is a .NET Standard library with the purpose of modeling song files for the video game Clone Hero. It currently supports reading of chart and ini files, with MIDI support planned after the first proper release which should arrive at some point in November.

## Installation
ChartTools currently has no proper release and must be imported using the project included in the repository.

To add ChartTools to your solution, select right-click on Dependeicnes of your project from the solution explorer and select "Add Project References...". Click on "Browse" and select ChartTools.csproj from your clone of the repository.

## Getting started
Every element of a chart is stored in an instance of the Song class. It can be initialized by reading a file. The file format is detected automatically using the extension:
```c#
Song song = Song.FromFile(filePath);
```
A song contains five main elements:

- Metadata - Miscelanious info about the song, such as title, album, charter etc.
- Sync track - Markers that define time signature and tempo
- Global events
- Instruments

Each of these elements can be read individually from a file using the non-generic version of the right class.

## Metadata
Similar to reading a song: metadata can be read from one or multiple files:
```c#
Metadata metadata = Metadata.FromFile(filePath);
Metadata metadata = Metada.FromFiles(filePath1, filePath2, filePath3...);
```
Files can be .chart or .ini. When reading from multiple files, the provided files can be a mix of different formats. Each file is read based on its extension.

## Instruments and tracks
All instrument currently supported are represented using the generic Insrument class. Notes and other information are stored in a track. Each generic instrument contains one track per difficulty setting. A track can be retrieved from a song like so:
```c#
Track<StandardChord> track = song.LeadGuitar.Expert;
```
Notice the use of StandardChord used as a generic type. Instrument are divided in three categories based on the type of chords it uses. These categories are:
- Standard - Five colored notes
- Drums - Five colored notes where open notes (kick) can mixed with regular notes
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
    chord.Notes.Add(new StandardNote(StandardNotes.Orange));
}
```

## Lyrics
Lyrics of a song are defined by a collection of phrase. A phrase represents a single line displayed on-screen. It contains syllables, each one representing a section of the text which changes color by the karaoke system. Phrases can either be read from a file:
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

lyrics.ToGlobalEvents(); //Creates a new set of global events
events.SetLyrics(lyrics); //Replaces existing lyric-related events with new events making up the phrases
```
## Optimizing
Although still fucntional, some files may contain data that slow down the reading process or in worse cases, may result in non-functionnal files when saved in certain formats. ChartTools provides various utilities to fix such issues:
```c#
using ChartTools.Optimization;

chords.CutSustains() //Cuts short sustains that exceed the position of the next identical note
chords.CutLengths() //Cuts short star power phrases that exceed the start of the next phrase

//Sorts and removes redundant markers
syncTrack.Tempo.RemoveUneeded();
syncTrack.TimeSignatures.RemoveUneeded();
```
## Writing files
Finally, changes can be saved to a file using the static ToFile method or either Metadata or Song. The format is based on the extension of the file. **NOTE: The file will be completely overwritten.**
```c#
song.ToFile(filePath); //.chart file only
metadata.ToFile(filePath) //.chart or .ini - some properties may not be written depending on the output format
```
