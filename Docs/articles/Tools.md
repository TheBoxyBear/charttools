# Tools
ChartTools includes multiple utility methods for performing common operations on charts. This guide will cover such utilities.

Fun fact: The name ChartTools comes from the original purpose of the library of only providing utility methods for charts.

## Accessing utilities
To start using utilities, add the `ChartTools.Tools` namespace to your usings. Once added, utilities can be called from their respective static class or as extension methods.

```csharp
using ChartTools.Tools;

AwesomeUtility.AwesomeUtility(song);
song.AwesomeUtility();
```

## Merging lengths
Some track objects define a length, such as special phrases and vocal phrases. `LengthMerger` takes a set of such objects and sets the first object in the sequence to cover the entire duration from the start position of the earliest object to the end position of the last.

```csharp
// T is the type of objects in the collection.
T modified = LengthMerger.MergeLengths<T>(longTrackObjects);
```

The object to modify can be changed by providing it as a target.

```csharp
target = LengthMerger.MergeLengths<T>(longTrackObjects, target);
```

If a sequence has an object covering positions 10-20 and another covering 22-30, the resulting object will have a start position of 10 and end position of 30.

## Cutting lengths and sustains
In some cases, tracks may be in a broken state where objects define lengths going part the start of the next matching objects. Such overlaps can be eliminated by cutting the lengths down to the start of the next matching object. Matching objects can be defined as any vocal phrases, special phrases of the same type and notes of the same lane or index.

### Cutting sustains
```csharp
List<StandardChord> ordered = Optimizer.CutSustains<StandardChord>(guitarChords);
```

Supports all chord types and simulates where sustains are foced to end due to another note of the same index or the presence of an open note and vice versa. Syllables are not modified, which may result in them falling outside the end position of the parent phrase. Part of the process of finding how to cut sustain involves ordering chords by position. The result of the ordering is provided as a return value.

If the chords are already ordered by position, the ordering can be skipped.

```csharp
List<StandardChord> ordered = Optimizer.CutSustains<StandardChord>(guitarChords, true);
```

### Cutting special phrases
```csharp
List<TrackSpecialPhrase>[] orderedGroups = Optimizer.CutSpecialLenghts<TrackSpecialPhrase>(phrases);
```

Groups phrases by special type and cuts shorts lenghts going over the next phrase of the same type. The grouping and ordering by position is provided as the return value, where each item in the array stores phrases of the same type.

If the phrases are already ordered, the ordering can be skipped with an optional parameter.

```csharp
List<TrackSpecialPhrase>[] orderedGroups = Optimizer.CutSpecialLenghts<TrackSpecialPhrase>(phrases, true);
```

> **NOTE**: Due to the grouping being based on the numeric value of the special type, only collections of instruments and track special phrases are supported. Using an `IEnumerable<SpecialPhrase>` will result in an exception.

### Cutting other long track objects
When the type of long objects is not known, a base method using the `ILongTrackObject` interface can be used. This method only applies the base logic, treating each object equally compared to how note sustains and special phrase lenghts which are grouped by their respective methods, returning the objects ordered by length.

```csharp
List<ILongTrackObject> Optimizer.CutLenghts<ILongTrackObject>(obejcts);
```

If the objects are already ordered, the ordering can be skipped with an optional parameter.

```csharp
List<ILongTrackObject> Optimizer.CutLenghts<ILongTrackObject>(obejcts, true);
```

# Removing redundant sync track markers
If tempo or time signature markers define the same value as the previous marker, their existence is redundant in gameplay and can be removed. The Optimizer class defines methods for this purpose.

```csharp
Optimizer.RemoveUneeded(tempoMarkers);
Optimizer.RemoveUneeded(timeSignatures);
```