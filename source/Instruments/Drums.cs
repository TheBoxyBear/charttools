using ChartTools.Animations;
using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record Drums : Instrument<DrumsChord>
{
    protected override InstrumentIdentity GetIdentity() => InstrumentIdentity.Drums;
    public override InstrumentType InstrumentType => InstrumentType.Drums;

    #region File reading
    [Obsolete($"Use {nameof(ChartFile.ReadDrums)}.")]
    public static Drums FromFile(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadDrums(path, config, formatting)));

    [Obsolete($"Use {nameof(ChartFile.ReadDrumsAsync)}.")]
    public static async Task<Drums> FromFileAsync(string path, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync<Drums>(path, (".chart", path => ChartFile.ReadDrumsAsync(path, config, formatting, cancellationToken)));

    [Obsolete($"Use {nameof(ChartFile.ReadDrums)} with {nameof(Metadata.Formatting)}.")]
    public static DirectoryResult<Drums> FromDirectory(string directory, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, config, formatting));

    [Obsolete($"Use {nameof(ChartFile.ReadDrumsAsync)} with {nameof(Metadata.Formatting)}.")]
    public static Task<DirectoryResult<Drums>> FromDirectoryAsync(string directory, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, config, formatting, cancellationToken), cancellationToken);

    internal override InstrumentMapper<DrumsChord> GetMidiMapper(WritingSession session, AnimationSet animations)
    {
        throw new NotImplementedException();
    }
    #endregion
}
