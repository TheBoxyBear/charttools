using ChartTools.Extensions.Linq;
using ChartTools.IO.Sources;
using ChartTools.Meta;

namespace ChartTools.IO.Ini;

/// <summary>
/// Provides methods for reading and writing ini files
/// </summary>
public static class IniFile
{
	/// <summary>
	/// Reads the <see cref="Metadata"/> from an ini target.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="existing"><see cref="Metadata"/> from another target to combine with</param>
	/// <returns><see cref="Metadata"/> object provided as the <paramref name="existing"/> parameter, or a new instance if passed <see langword="null"/>.</returns>
	public static Metadata ReadMetadata(ReadingDataSource source, Metadata? existing = null)
	{
		using IniFileReader reader = new(source, existing);
		reader.Read();

		return reader.Parsers.TryGetFirst(out IniParser? parser)
			? parser.Result
			: throw SectionException.MissingRequired(IniFormatting.Header);
	}

	/// <summary>
	/// Reads the <see cref="Metadata"/> from an ini target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to read from</param>
	/// <param name="existing"><see cref="Metadata"/> from another target to combine with</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	/// <returns><see cref="Metadata"/> object provided as the <paramref name="existing"/> parameter, or a new instance if passed <see langword="null"/>.</returns>
	public static async Task<Metadata> ReadMetadataAsync(
		ReadingDataSource source, Metadata? existing = null, CancellationToken cancellationToken = default)
	{
		using IniFileReader reader = new(source, existing);
		await reader.ReadAsync(cancellationToken);

		return reader.Parsers.TryGetFirst(out IniParser? parser)
			? parser.Result
			: throw SectionException.MissingRequired(IniFormatting.Header);
	}

	/// <summary>
	/// Writes the <see cref="Metadata"/> to an ini target.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="metadata"><see cref="Metadata"/> to write</param>
	public static void WriteMetadata(WritingDataSource source, Metadata metadata)
	{
		using IniFileWriter writer = new(source, new IniSerializer(metadata));
		writer.Write();
	}

	/// <summary>
	/// Writes the <see cref="Metadata"/> to an ini target asynchronously.
	/// </summary>
	/// <param name="source">File path or stream to write to</param>
	/// <param name="metadata"><see cref="Metadata"/> to write</param>
	/// <param name="cancellationToken">Token used for cancellation</param>
	public static Task WriteMetadataAsync(WritingDataSource source, Metadata metadata, CancellationToken cancellationToken = default)
	{
		using IniFileWriter writer = new(source, new IniSerializer(metadata));
		return writer.WriteAsync(cancellationToken);
	}
}
