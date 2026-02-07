# Dynamic Syntax
This guide will cover an alternate and more flexible syntax for accessing components.

## Using the dynamic syntax
ChartTools supports a dynamic syntax to retrieve instruments and tracks using identity enums instead of explicit properties.

```csharp
StandardInstrument guitar = song.Instruments.Get(StandardInstrumentIdentity.LeadGuitar);
Instrument bass = song.Instruments.Get(InstrumentIdentity.StandardBass);

Track<StandardChord> easyGuitar = guitar.GetTrack(Difficulty.Easy);
Track easyBass = bass.GetTrack(Difficulty.Easy);
```

The dynamic syntax uses three enums to get instruments:

- [StandardInstrumentIdentity](~/api/ChartTools.StandardInstrumentIdentity.yml) - Instruments using standard chords
- [GHLInstrumentIdentity](~/api/ChartTools.GHLInstrumentIdentity.yml) - Instruments using Guitar Hero Live chords
- [InstrumentIdentity](~/api/ChartTools.InstrumentIdentity.yml) - All instruments including drums

Drums do not an enum for their chord types as they are the only instrument using their respective chords.

## Generic vs. non-generic
When an instrument is obtained dynamically using the [InstrumentIdentity](~/api/ChartTools.InstrumentIdentity.yml) enum, the returned object is of type [Instrument](~/api/ChartTools.Instrument.yml). When a track is obtained from a non-generic instrument, either dynamically or explicitly through a property, the track will be of type [Track](~/api/ChartTools.Trac.yml). This concept extends to chords and notes.

When working with a non-generic track, the following rules apply:
- Chords cannot be added or removed. The position of existing chords can be modified.
- Notes can be created using `CreateNote`
- A note's identity can be obtained through the read-only [Index](~/api/ChartTools.INote#ChartTools_INote_Index.yml) property.
- A note's sustain can be modified.
- Local events and special phrases have no restrictions.

Being the base types of the generic counterparts, non-generic instruments, tracks, chords and notes can be cast to a generic version:

```csharp
Instrument instrument = song.Instruments.Get(InstrumentIdentity.StandardLeadGuitar);
StandardInstrument stadardInstrument = (StandardInstrument)instrument;

Track track = instrument.GetTrack(Difficulty.Easy);
Track<StandardChord> standardTrack = (Track<StandardChord>)track;

Chord chord = track.Chords[0];
StandardChord standardChord = (StandardChord)chord;

INote note = chord.Notes[0];
LaneNote<StandardLane> standardNote = (LaneNote<StandardLane>)note;
```

The dynamic syntax can also be used to set and read instruments and tracks.

```csharp
// Setting components
song.Instruments.Set(guitar);
song.Instruments.Set(guitar with { InstrumentIdentity = StandardInstrumentIdentity.Bass });

song.Instruments.StandardLeadGuitar.SetTrack(new() { Difficulty = Difficulty.Easy });
```

When setting an instrument, the target is determined by the [InstrumentIdentity](~/api/ChartTools.Instrument#ChartTools_Instrument_InstrumentIdentity.yml) property of the new instrument, which can be overridden using a `with` statement. Similarly, the target difficulty when setting a track is determined by the track's [Difficulty](~/api/ChartTools.Track#ChartTools_Track_Difficulty.yml) property, also overridable through `with`. 

