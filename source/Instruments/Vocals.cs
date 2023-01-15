using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Formatting;
using ChartTools.Lyrics;

namespace ChartTools;

public record Vocals : Instrument<Phrase>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Vocals;

    #region File reading
    /// <summary>
    /// Reads vocals from a file
    /// </summary>
    public static Vocals? FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadVocals(path)));
    /// <summary>
    /// Reads vocals from a file asynchronously using multitasking.
    /// </summary>
    public static async Task<Vocals?> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Vocals?>(path, (".chart", path => ChartFile.ReadVocalsAsync(path)));
    #endregion
}
