namespace ChartTools.IO.Sources;

/// <summary>
/// Represents a source of data to read from that can be used as a <see cref="Stream"/>.
/// </summary>
/// <remarks>Can be implicitly converted from a <see cref="string"/> or <see cref="Stream"/>.</remarks>
public class ReadingDataSource : DataSource
{
	/// <summary>
	/// Creates a <see cref="ReadingDataSource"/> from a <see cref="Stream"/>.
	/// </summary>
	/// <param name="stream">Stream to source from. Must be readable through <see cref="Stream.CanRead"/>.</param>
	/// <exception cref="ArgumentException">The stream is not readable.</exception>
	/// <remarks>The stream is kept alive after the lifetime of the <see cref="ReadingDataSource"/>.</remarks>
	public ReadingDataSource(Stream stream) : base(stream)
	{
		if (!stream.CanRead)
			throw new ArgumentException("Stream is unable to be read.", nameof(stream));
	}

	/// <summary>
	/// Creates a <see cref="ReadingDataSource"/> from a file path.
	/// </summary>
	/// <param name="path">Path of the file to source from</param>
	/// <remarks>
	/// <para>Initializes the stream as a <see cref="FileStream"/> with <see cref="FileAccess.Read"/> and <see cref="FileShare.Write"/>.</para>
	/// <para>The stream is disposed when disposing the <see cref="ReadingDataSource"/>.</para>
	/// </remarks>
	public ReadingDataSource(string path) : base(path, FileMode.Open, FileAccess.Read, FileShare.Write) { }

	/// <inheritdoc cref="ReadingDataSource(Stream)"/>
	public static implicit operator ReadingDataSource(Stream stream) => new(stream);

	/// <inheritdoc cref="ReadingDataSource(string)"/>
	public static implicit operator ReadingDataSource(string path) => new(path);
}
