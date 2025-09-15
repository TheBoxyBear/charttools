# Configuration

Read and write operations in ChartTools feature customizable behavior for how they handle various error cases. These behaviors can be defines throguh the use of configuration objects.

The following snippet uses a [ChartReadingConfiguration](~/api/ChartTools.IO.Configuration.Chart.ChartReadingConfiguration.yml) to only include the first of duplicate [track objects](~/api/ChartTools.ITrackObject.yml):

```csharp
using ChartTools.IO.Chart.Configuration;

Song song = ChartFile.ReadSong("notes.chart", new ChartReadingConfiguration()
{
	DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.IncludeFirst
});
```

When working with a non-specified file format, a [ReadingConfiguration](~/api/ChartTools.IO.Configuration.ReadingConfiguration) or [WritingConfiguration](~/api/ChartTools.IO.Configuration.WritingConfiguration) is used instead. These configuration envelop multiple configurations for the various file formats.

Here is the same example as above, but using a `Song.FromFile`:

```csharp
using ChartTools.IO.Configuration;

Song song = Song.FromFile("notes.chart", new ReadingConfiguration()
{
	Chart = new ChartReadingConfiguration()
	{
		DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.IncludeFirst
	}
});
```

## Default configurations
Each file format defines default configurations for properties with no value specified or when no configuration object is provided. These defaults can be overwritten through the respective file class.

In this ecxample, replacing the default prior to reading the song has the same effect as the original example:

```csharp
using ChartTools.IO.Chart.Configuration;

ChartFile.DefaultReadConfig = ChartFile.DefaultReadConfig with
{
	DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.IncludeFirst
};

Song song = ChartFile.ReadSong("notes.chart");
```

> [!NOTE]
> To prevent breaking ongoing read/write operations, configurations are immutable. Hence, the above snippet uses a `with` expression to create a new configuration based on the default.

