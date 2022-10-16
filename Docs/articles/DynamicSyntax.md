# Dynamic Syntax
This guide will cover an alternate and more flexible syntax for accessing components.

## Using the dynamic syntax
ChartTools supports a dynamic syntax to retrieve instruments and tracks using identity enums instead of explicit properties.

```C#
StandardInstrument guitar = song.Instruments.Get(StandardInstrumentIdentity.LeadGuitar);
Instrument bass = song.Instruments.Get(InstrumentIdentity.Bass);

Track<StandardChord> easyGuitar = guitar.GetTrack(Difficulty.Easy);
Track easyBass = bass.GetTrack(Difficulty.Easy);
```

The dynamic syntax uses three enums to get instruments:

- `StandardInstrumentIdentity` - Instruments using standard chords
- `GHLInstrumentIdentity` - Instrument using Guitar Hero Live chords
- `InstrumentIdentity` - All instruments including drums and vocals

Drums and vocals do not have an enum for their chord types as they are the only instrument using their respective chords.

## Generic vs. non-generic
When an instrument is obtained dynamically using the `InstrumentIdentity` enum, the returned object is of type `Instrument`. When a track is obtained from a non-generic instrument, either dynamically or explicitly through a property, the track will be of type `Track`. This concept extends to chords and notes.

When working with a non-generic track, the following rules apply:
- Chords cannot be added or removed. The position of existing chords can be modified.
- Local events and special phrases have no restrictions.
- A note's identity can be obtaines through the read-only `NoteIndex` property. In a future version, notes will be able to be added from a non-generic track.

Being the base types of the generic counterparts, non-generic instruments, tracks, chords and notes can be cast to a generic version.

The dynamic syntax can also be used to set amd read instruments and tracks.

```C#
// Setting components
song.Instruments.Set(guitar);
song.Instruments.Set(guitar with { InstrumentIdentity = InstrumentIdentity.Bass });

song.Instruments.LeadGuitar.SetTrack(new() { Difficulty = Difficulty.Easy });

// Reading components
StandardInstrument coop = StandardInstrument.FromFile(path, StandardInstrumentIdentity.CoopGuitar, <ReadingConfiguration>, metadata.Formatting);
Instrument keys = Instrument.FromFile(path, InstrumentIdentity.Keys, <ReadingConfiguration>, metadata.Formatting);

Track<StandardChord> easyCoop = Track.FromFile(path, StandardInstrumentIdentity.CoopGuitar, Difficulty.Easy, <ReadingConfiguration>, metadata.Formatting);
Track easyKeys = Track.FromFile(path, InsturmentIdentity.Keys, Difficulty.Easy, <ReadingConfiguration>, metadata.Formatting);
```

When setting an instrument, the target is determined by the `InstrumentIdentity` property of the new instrument, which can be overridden using a `with` statement. Similarly, the target difficulty when setting a track is determined by the track's `Difficulty` property, also overridable through `with`. 

> **NOTE**: Unlike when setting an instrument explicitely, the existing identity is used when setting dynamically. This makes it safe to reuse the previous reference after the assignement unless a `with` statement is used. Tracks still need to be re-obtained when using the dynamic syntax as a copy is created to assign its `ParentInstrument`. In cases where a reference to an instrument or track needs to be re-obtained, this reference is passed through as the return of `InstrumentSet.Set` and `Instrument.SetTrack`.
