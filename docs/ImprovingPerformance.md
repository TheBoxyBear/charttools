# Improving performance

This guide will cover alternate tecniques that will improve performance when using ChartTools.

### Single components

Rather than performing IO operation on entire songs, such operations can be made on individual components.

```c#
(Instrument<StandardChord> guitar, Metadata metadata) = Insrument.FromDirectory(directory, StandardInstrumentIdentity.LeadGuitar);
guitar.ToFile(path, <WritingConfiguration>, metadata.Formatting);
```

When writing a component to an existing file, the parts of the file regarding the component will be modified.

When reading a component of a chart whose metadata has already been read, you may use the `FromFile` method along with the existing formatting.

```c#
Instrument<StandardChord> guitar = Instrument.FromFile(path, StandardInstrumentIdentity.LeadGuitar, <ReadingConfiguration>, metadata.Formatting);
```

### Asynchronous operations

Every IO operation can be performed asynchronously by appending `Async` to the name of a method.

```c#
Task<Song> readTask = Song.FromDirectoryAsync(directory);
```

Asynchronous operations support a `CancellationToken` as an optional parameter. If omitted. `CancellationToken.None` will be used. Writing operations make use of a temporary file and can be safely canceled without file corruption.

```c#
Task<Song> readTask = Song.FromDirectoryAsync(directory, <ReadingConfiguration>, cancellationToken);
```

The asynchronous operations make heavy use of multi-threading and are beneficial even if the result is to be awaited immediately.

### Targeted formats

By default, the target format of an IO operation is determined by the file extension. You can bypass the extension check by using the file classes located in `ChartTools.IO`.

```c#
Song song = ChartFile.ReadSong(path);
Metadata metadata = IniFile.ReadMetadata(path);
```
