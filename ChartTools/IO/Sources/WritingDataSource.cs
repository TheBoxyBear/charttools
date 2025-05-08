namespace ChartTools.IO.Sources;

public class WritingDataSource : DataSource
{
	public ReadingDataSource? Existing { get; }

	public WritingDataSource(Stream stream, ReadingDataSource? existing = null) : base(stream)
	{
		if (stream.CanSeek || stream.CanWrite)
			throw new ArgumentException("Stream is not seekable or writable", nameof(stream));

		Existing = existing;
	}

	public WritingDataSource(string path, ReadingDataSource? existing = null)
		: base(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read)
		=> Existing = existing;

	public override void Dispose()
	{
		base.Dispose();
		Existing?.Dispose();
	}

	/// <summary>
	/// Creates a <see cref="WritingDataSource"/> from a <see cref="Stream"/> as a target and source of existing data.
	/// </summary>
	/// <param name="stream">Stream to create from</param>
	public static implicit operator WritingDataSource(Stream stream) => new(stream, stream);


	/// <summary>
	/// Creates a <see cref="WritingDataSource"/> from a file path as a target and source of existing data.
	/// </summary>
	/// <param name="path">Path to create from</param>
	public static implicit operator WritingDataSource(string path) => new(path, path);
}
