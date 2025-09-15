namespace ChartTools.IO.Sources;

/// <summary>
/// Represents a combination of a data source that can be written to and a <see cref="ReadingDataSource"/> to combine data with.
/// </summary>
/// <remarks>Can be implicitly converted from a <see cref="string"/> or <see cref="Stream"/>.</remarks>
public class WritingDataSource : DataSource
{
	/// <summary>
	/// Source of existing data to combine during write operations
	/// </summary>
	/// <remarks>Can point to the same file or share a <see cref="Stream"/> instance with the <see cref="WritingDataSource"/>.</remarks>
	public ReadingDataSource? Existing { get; }

	/// <summary>
	/// Creates a <see cref="WritingDataSource"/> from a <see cref="Stream"/>.
	/// </summary>
	/// <param name="stream">Stream to source from. Must be readable through <see cref="Stream.CanRead"/> and seekable through <see cref="Stream.CanSeek"/>.</param>
	/// <param name="existing"><inheritdoc cref="Existing" path="/summary"/></param>
	/// <exception cref="ArgumentException">The stream is not seekable and/or writable</exception>
	/// <remarks>The stream is kept alive after the lifetime of the <see cref="WritingDataSource"/>.</remarks>
	public WritingDataSource(Stream stream, ReadingDataSource? existing = null) : base(stream)
	{
		if (!stream.CanSeek || !stream.CanWrite)
			throw new ArgumentException("Stream is not seekable or writable", nameof(stream));

		Existing = existing;
	}

	/// <summary>
	/// Creates a <see cref="WritingDataSource"/> from a file path.
	/// </summary>
	/// <param name="path">Path of the file to source from</param>
	/// <param name="existing"><inheritdoc cref="Existing" path="/summary"/>. If <see langword="null"/>, uses the writing path if the file already exists.</param>
	/// <remarks>
	/// <para>Initializes the stream as a <see cref="FileStream"/> with <see cref="FileAccess.Write"/> and <see cref="FileShare.Read"/>.</para>
	/// <para>The stream is disposed when disposing the <see cref="WritingDataSource"/>.</para>
	/// </remarks>
	public WritingDataSource(string path, ReadingDataSource? existing = null)
		: base(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
	{
		Existing = existing is not null ? existing :
			(File.Exists(path) ? new(path) : null);
	}

	/// <summary>
	/// Disposes the reading source if provided and stream if generated.
	/// </summary>
	public override void Dispose()
	{
		base.Dispose();
		Existing?.Dispose();
	}

	/// <summary>
	/// Creates a <see cref="WritingDataSource"/> from a <see cref="Stream"/> as a target and source of existing data.
	/// </summary>
	/// <param name="stream">Stream to create from</param>
	/// <inheritdoc cref="WritingDataSource(Stream, ReadingDataSource?)" path="/remarks"/>
	public static implicit operator WritingDataSource(Stream stream) => new(stream, stream);

	/// <summary>
	/// Creates a <see cref="WritingDataSource"/> from a file path as a target and source of existing data.
	/// </summary>
	/// <param name="path">Path to create from</param>
	/// <inheritdoc cref="WritingDataSource(string, ReadingDataSource?)" path="/remarks"/>
	public static implicit operator WritingDataSource(string path) => new(path, path);
}
