# Improving Performance
This guide will cover alternate techniques that will improve performance when using ChartTools.

## Configuration
By default, IO operations make multiple integrity checks to resolve errors. These checks can be configured or skipped by using a [Readingconfiguration](~/api/ChartTools.IO.Configuration.ReadingConfiguration.yml) or [WritingConfiguration](~/api/ChartTools.IO.Configuration.WritingConfiguration.yml) object. [Learn more about configuring IO operations](~/articles/configuration.md).

The following example reads a song while bypassing checks for duplicate track objects:

```csharp
using ChartTools.IO.Configuration;

Song.FromFile("notes.chart", new ReadingConfiguration { Chart = new() { DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.IncludeAll } });
```

## Targeted formats
By default, the target format of an IO operation is determined by the file extension. You can bypass the extension check by using the respective file class located under [ChartTools.IO](~/api/ChartTools.IO.yml).

```csharp
using ChartTools.IO.Chart;
using ChartToole.IO.Ini;

Song song = ChartFile.ReadSong("notes.chart");
Metadata metadata = IniFile.ReadMetadata("song.ini");
```

When working with a specific format, the file path can be swapped for a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream).

```csharp
Stream fs = File.Open("notes.chart", FileMode.Open, FileAccess.Read);
Song song = ChartFile.ReadSong(fs);
```

## Single components
Rather than performing IO operation on entire songs, such operations can be made on individual components. When writing a component to an existing file, the parts of the file regarding the component will be modified.

```csharp
Metadata metadata = IniFile.ReadMetadata("song.ini");
SyncTrack guitar = ChartFile.ReadSyncTrack("notes.chart");
```

### Component lists
If multiple components are needed, they can be combined in a single operation using a [ComponentList](~/api/ChartTools.IO.Components.ComponentList.yml).

```csharp
using ChartTools.IO.Chart;
using ChartTools.IO.Components;

Song song = ChartFile.ReadComponents("notes.chart", new ComponentList()
{
	Metadata     = true,
	SyncTrack    = true,
	GlobalEvents = true,
	Instruments  = new InstrumentComponentList()
	{
		StandardLeadGuitar = DifficultySet.All,
		StandardBass = DifficultySet.Easy | DifficultySet.Expert
	}
});
```

## Asynchronous operations
Every IO operation can be performed asynchronously by appending `Async` to the name of a method.

```charp
Task<Song> readTask = Song.FromDirectoryAsync(directory);
```

Asynchronous operations support a [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) as an optional parameter. If omitted. [CancellationToken.None](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken.none#system-threading-cancellationtoken-none) will be used.

```csharp
Task<Song> readTask = Song.FromDirectoryAsync(directory, <ReadingConfiguration>, <CancellationToken>);
```

Asynchronous operations make heavy use of multi-threading and are beneficial even if the result is to be awaited immediately.