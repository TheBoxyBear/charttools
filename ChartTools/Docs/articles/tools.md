# Tools
ChartTools includes multiple utility methods for performing common operations on charts. This guide will cover such utilities.

Fun fact: The name ChartTools comes from the original purpose of the library of only providing utility methods for charts.

## Accessing tools
To start using tools, add the [ChartTools.Tools](~/api/ChartTools.Tools.yml) namespace to your usings. Once added, tools can be called from their respective static class or as extension methods.

```csharp
using ChartTools.Tools;

AwesomeUtility.AwesomeTool(song);
song.AwesomeTool();
```

## Cutting lengths and sustains
In some cases, tracks may be in a broken state where objects define lengths going part the start of the next matching objects. The Optimizer class provides methods for fixing such overlaps by cutting short lengths going past the start of certain objects. This process involves ordering the objects by position, the result of which is provided as a return value. If the objects are known to already be in order, the ordering can be skipped with an optional parameter.

### Cutting sustains
```csharp
List<StandardChord> ordered = Optimizer.CutSustains<StandardChord>(guitarChords, <skipOrdering>);
```

Supports all chord types and simulates where sustains are forced to end due to another note of the same index or the presence of an open note and vice versa. Syllables are not modified, which may result in them falling outside the end position of the parent phrase. Part of the process of finding how to cut sustain involves ordering chords by position. The result of the ordering is provided as a return value.

### Cutting special phrases
```csharp
List<TrackSpecialPhrase>[] orderedGroups = Optimizer.CutSpecialLenghts<TrackSpecialPhrase>(phrases, <skipOrdering>);
```

Groups phrases by special type before applying the cutting to each group individually. The grouping and ordering by position is provided as the return value, where each item in the array stores phrases of the same type.

> **NOTE**: Due to the grouping being based on the numeric value of the special type, only collections of instruments and track special phrases are supported. Using an `IEnumerable<SpecialPhrase>` will result in an exception.

### Cutting other long track objects
When the type of long objects is not known, a base method using the [ILongTrackObject](~/api/ChartTools.ILongTrackObject.yml) interface can be used. This method only applies the base logic, treating each object equally compared to how note sustains and special phrase lengths which are grouped by their respective methods, returning the objects ordered by length.

```csharp
List<ILongTrackObject> oredredObjects = Optimizer.CutLenghts<ILongTrackObject>(objects, <skipOrdering>);
```

## Removing redundant sync track markers
The [Optimizer](~/api/ChartTools.Tools.Optimizer.yml) class provides methods for removing tempo and time signature markers that have no effect on gameplay. Like with length cutting, the objects are ordered by position and provided as a return value. The ordering can also be skipped with an optional parameter.

```csharp
List<TimeSignature> orderedSignatures = Optimizer.RemoveUneeded(tempoMarkers, <skipOrdering>);
List<Tempo> orderedTempos = Optimizer.RemoveUneeded(timeSignatures, <skipOrdering>);
```

### Anchored tempos
The method for removing tempo markers cannot work with anchored markers for which the tempo position is desynchronized and will throw an exception. An overload is provided which calculates the tick positions for such markers. This overload requires the temporal resolution obtained from the song's formatting. The optional parameter for skipping the ordering only applies to the ordering of synchronized markers.

```csharp
List<Tempo> orderedMarkers = Optimizer.RemoveUneeded(tempoMap, resolution, <skipSyncedOrdering>);
```

## Tempo rescaling
The [TempoRescaler](~/api/ChartTools.Tools.TempoRescaler.yml) class provides scaling methods for various groups of track objects.

```csharp
TempoRescaler.Rescale(longObject, scale);
TempoRescaler.Rescale(trackObject, scale);
TempoRescaler.Rescale(longTrackObject, scale)
TempoRescaler.Rescale(tempo, scale);
TempoRescaler.Rescale(chord, scale);
TempoRescaler.Rescale(track, scale);
TempoRescaler.Rescale(instrument, scale);
TempoRescaler.Rescale(syncTrack, scale);
TempoRescaler.Rescale(song, scale);
```
