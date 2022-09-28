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

## Utilities
### Length merging
Some track objects define a length, such as special phrases and vocal phrases. `LengthMerger` takes a set of such objects and sets the first object in the sequence to cover the entire duration from the start position of the earliest object to the end position of the last.

```csharp
// T is the type of objects in the collection.
T modified = LengthMerger.MergeLengths<T>(longTrackObjects);
T modified = longTrackObjects.MergeLenghts<T>();
```

The object to modify can be changed by providing it as a target.

```csharp
target = LengthMerger.MergeLengths<T>(longTrackObjects, target);
```

If a sequence has an object covering positions 10-20 and another covering 22-30, the resulting object will have a start position of 10 and end position of 30.

## Cutting lengths and sustains
In some cases, tracks may be in a broken state where objects define lengths going part the start of the next matching objects. Such overlaps can be eliminated by cutting the lengths down to the start of the next matching object. Matching objects can be defined as any vocal phrases, special phrases of the same type and notes of the same lane or index.

```csharp
Optimizer.CutLengths(vocals.Expert.Chords); // Syllables are not modified. The new end position of the phrase might result in syllable bieng missing in-game.
Optimizer.CutLengths(specialPhrases); // Cuts the lengths by considering phrases of the same type.
Optimizer.CutSustains(laneChords); // Cuts the sustains of each note by considering notes of the same index.
```

> **NOTE**: When cutting lengths of special phrases, type grouping will only take place if the collection is of type SpecialPhrase or a derived type. Mixing instruments and track special phrases will result in the grouping being based on the numeric value of the type.

