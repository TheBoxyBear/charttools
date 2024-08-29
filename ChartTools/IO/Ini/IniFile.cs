using ChartTools.Extensions.Linq;
using ChartTools.IO.Formatting;
using ChartTools.IO.Configuration;
using ChartTools.IO.Sources;

namespace ChartTools.IO.Ini;

/// <summary>
/// Provides methods for reading and writing ini files
/// </summary>
public static class IniFile
{
    /// <inheritdoc cref="Metadata.FromFile(string)"/>
    /// <param name="path"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
    /// <returns>A new instance of <see cref="Metadata"/> if <paramref name="existing"/> is <see langword="null"/>, otherwise the same reference.</returns>
    public static Metadata ReadMetadata(string path, Metadata? existing = null)
    {
        var reader = new IniFileReader(new(path), existing);
        reader.Read();

        return reader.Parsers.TryGetFirst(out var parser)
            ? parser!.Result
            : throw SectionException.MissingRequired(IniFormatting.Header);
    }
    /// <inheritdoc cref="Metadata.FromFile(string)"/>
    /// <param name="path"><inheritdoc cref="Song.FromFile(string, ReadingConfiguration?, FormattingRules?)" path="/param[@name='path']"/></param>
    /// <returns>A new instance of <see cref="Metadata"/> if <paramref name="existing"/> is <see langword="null"/>, otherwise the same reference.</returns>
    public static async Task<Metadata> ReadMetadataAsync(string path, Metadata? existing = null, CancellationToken cancellationToken = default)
    {
        var reader = new IniFileReader(new(path), existing);
        await reader.ReadAsync(cancellationToken);

        return reader.Parsers.TryGetFirst(out var parser)
            ? parser!.Result
            : throw SectionException.MissingRequired(IniFormatting.Header);
    }

    /// <summary>
    /// Writes the metadata in a file.
    /// </summary>
    /// <param name="path">Path of the file to read</param>
    /// <param name="metadata">Metadata to write</param>
    public static void WriteMetadata(string path, Metadata metadata)
    {
        using var source = new WritingDataSource(path);

        var writer = new IniFileWriter(source, new IniSerializer(metadata));
        writer.Write();
    }
}
