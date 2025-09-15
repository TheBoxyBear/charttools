# Lyrics
ChartTools supports lyrics in the form of the [Vocals](~/api/ChartTools.Lyrics.Vocals) component. This component is split into tracks for standard vocals and harmonics, although only standard vocals are currently supported. Unlike other instruments, vocals are not split into tracks based on difficulty. Rather, all difficulties use the same note data, with difficulty being driven by how each game registers a note being sung correctly. Additionally, as vocals have no concept of chords, notes are instead defined as collections of pitches grouped in phrases.

## Vocals notes
Unlike other note types, vocals notes are defined by a position, pitch, and length.

Notes support the pitches in the range C2 to C6, represented by the [VocalPitchValue](~/api/ChartTools.Lyrics.VocalPitchValue.yml) enum. The enum uses a binary representation to isolate keys and octaves while staying true to music theory when comparing values.

```csharp
using System.Diagnostics;
using ChartTools.Lyrics;

Debug.Assert(VocalsPitchValue.C2 < VocalsPitchValue.C3);
Debug.Assert(VocalsPitchValue.C2 < VocalsPitchValue.D2);
Debug.Assert(VocalsPitchValue.CSharp2 == VocalsPitchValue.Db2);
```

To help work with the large range of possible pitches, ChartTools provides the [VocalsPitch](~/api/ChartTools.Lyrics.VocalsPitch.yml) weapper struct, which provides helper properties for the key and octave. This struct is fully interchangable with the enum.

```csharp
using System.Diagnostics;
using ChartTools.Lyrics;

VocalsPitch pitch = VocalsPitchValue.DSharp2;

Debug.Assert(pitch.Octave == 2);
Debug.Assert(pitch.Key == VocalsKey.DSharp);
Debug.Assert(pitch.Key == VocalsKey.Eb);

VocalsPitchValue value = pitch;
```

> [!NOTE]
> Because some enum values for sharps and flats have identical numeric values to honor music theory, displaying such values in a debugger or as a string conversion can lead to a different value that the one assigned.
> ```csharp
> using System.Diagnostics;
> using ChartTools.Lyrics;
> 
> Debug.Assert(VocalsPitchValue.Db2.ToString() == "Db2");
> Debug.Assert(VocalsPitchValue.CSharp2.ToString() == "Db2");
> Debug.Assert(VocalsPitchValue.CSharp2.ToString() != "CSharp2");
> ```

## Lyrics in chart files
As the `.chart` format does not formaly support playable vocals, lyrics are represented trough [global events](~/articles/events.md) rather than notes. When reading from a `.chart` file, lyric data will only be read if either the [GlobalEvents](~/api/ChartTools.Components.ComponentList.yml#ChartTools_IO_Components_InstrumentComponentList_GlobalEvents) or [Vocals](~/api/ChartTools.Components.ComponentList.yml#ChartTools_IO_Components_InstrumentComponentList_Vocals) component is enabled, after which the data will be stored in the corresponding location(s) in the song object. If accessed through vocals, notes will have the special pitch value of [None](~/api/ChartTools.Lyrics.VocalsPitchValue.yml#ChartTools_Lyrics_VocalsPitchValue_None), as global events do not define pitch.

Lyrics can also be converted to and from global events.

```csharp
using ChartTools.Lyrics;

List<GlobalEvent> events = ChartFile.ReadGlobalEvents("notes.chart");
events.GetLyrics(out IList<PhraseMarker> markers, out IList<VocalsNote> notes);
StandardVocalsTrack track = new(markers, notes);

events.SetLyrics(markers, notes);
events.SetLyrics(track);

events = [..track.ToGlobalEvents()];
```
> [!WARNING]
> When writing to a `.chart` file with both components enabled, any global event related to lyrics will be removed and replaced with the data from vocals.

## Phrases
Because certain lyric tracks share [phrase markers](~/api/ChartTools.Vocals/PhraseMarker.yml), notes and phrase markers are stored as separate collections rather than grouping notes under phrases. To simplify working with lyrics, these collections can be promoted to a set of [phrases](~/api/ChartTools.Lyrics.Phrase.yml).

The following snippet uses phrases to print the lyrics to a song:

```csharp
using ChartTools.Lyrics;

Song song = Song.FromFile("notes.chart");

foreach (Phrase phrase in song.Vocals.Standard.GetLyrics())
	Console.WriteLine(phrase.DisplayedText);
```

While part of a phrase, notes cannot be added or removed, but existing notes can be mutated, and the position and length of the phrase can be adjusted, reflecting the changes in the phrase marker. As such, phrases do not need to be converted back into notes and phrase markers. However, the conversion is needed if converting directly from global events to phrases.