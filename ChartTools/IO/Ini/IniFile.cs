using ChartTools.Extensions.Linq;
using ChartTools.IO.Sources;

namespace ChartTools.IO.Ini;

/// <summary>
/// Provides methods for reading and writing ini files
/// </summary>
public static class IniFile
{
	public static Metadata ReadMetadata(ReadingDataSource source, Metadata? existing = null)
	{
		using IniFileReader reader = new(source, existing);
		reader.Read();

		return reader.Parsers.TryGetFirst(out IniParser? parser)
			? parser.Result
			: throw SectionException.MissingRequired(IniFormatting.Header);
	}

	public static async Task<Metadata> ReadMetadataAsync(
		ReadingDataSource source, Metadata? existing = null, CancellationToken cancellationToken = default)
	{
		using IniFileReader reader = new(source, existing);
		await reader.ReadAsync(cancellationToken);

		return reader.Parsers.TryGetFirst(out IniParser? parser)
			? parser.Result
			: throw SectionException.MissingRequired(IniFormatting.Header);
	}

	public static void WriteMetadata(WritingDataSource source, Metadata metadata)
	{
		using IniFileWriter writer = new(source, new IniSerializer(metadata));
		writer.Write();
	}
}
