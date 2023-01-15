using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;

namespace ChartTools;

public record Drums : Instrument<DrumsChord>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Drums;

    #region File reading
    /// <summary>
    /// Reads drums from a file.
    /// </summary>
    public static Drums? FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadDrums(path, config, formatting)));
    /// <summary>
    /// Reads drums from a file asynchronously using multitasking.
    /// </summary>
    public static async Task<Drums?> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Drums?>(path, (".chart", path => ChartFile.ReadDrumsAsync(path, config, formatting, cancellationToken)));

    public static DirectoryResult<Drums?> FromDirectory(string directory, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, config, formatting));
    public static Task<DirectoryResult<Drums?>> FromDirectoryAsync(string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, config, formatting, cancellationToken), cancellationToken);
    #endregion
}
