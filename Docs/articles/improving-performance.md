# Improving Performance
This guide will cover alternate techniques that will improve performance when using ChartTools.

## Single components
Rather than performing IO operation on entire songs, such operations can be made on individual components. When writing a component to an existing file, the parts of the file regarding the component will be modified.

```c#
Metadata metadata = Metadata.FromFile(path);
StandardInstrument guitar = ChartFile.ReadInstrument(path, <WritingConfiguration>, metadata.Formatting);
```

> **NOTE**: Due to complications with implementing Midi support, operations on single instruments and tracks through their respective class have been deprecated. These operations must now be performed through the respective format class such as ChartFile.

## Configuration
By default, IO operations make multiple integrity checks to resolve errors. These checks can be configured or skipped by using a `Configuration` object. [Learn more about configuring IO operations](Configuration.md).

```csharp
Song song = Song.FromDirectory(directory, new ReadingConfiguration { DuplicateTrackObjectPolicy = DuplicateTrackObjectPolicy.IncludeAll });
```

## Asynchronous operations
Every IO operation can be performed asynchronously by appending `Async` to the name of a method.

```c#
Task<Song> readTask = Song.FromDirectoryAsync(directory);
```

Asynchronous operations support a `CancellationToken` as an optional parameter. If omitted. `CancellationToken.None` will be used. Writing operations make use of a temporary file and can be safely canceled without file corruption.

```c#
Task<Song> readTask = Song.FromDirectoryAsync(directory, <ReadingConfiguration>, <CancellationToken>);
```

The asynchronous operations make heavy use of multi-threading and are beneficial even if the result is to be awaited immediately.

## Targeted formats
By default, the target format of an IO operation is determined by the file extension. You can bypass the extension check by using the file classes located in `ChartTools.IO`.

```c#
Song song = ChartFile.ReadSong(path);
Metadata metadata = IniFile.ReadMetadata(path);
```
