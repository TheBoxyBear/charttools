using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

public record GHLInstrument : Instrument<GHLChord>
{
    public new GHLInstrumentIdentity InstrumentIdentity { get; init; }
    public override InstrumentType InstrumentType => InstrumentType.GHL;

    public GHLInstrument() { }
    public GHLInstrument(GHLInstrumentIdentity identity)
    {
        Validator.ValidateEnum(identity);
        InstrumentIdentity = identity;
    }

    protected override InstrumentIdentity GetIdentity() => (InstrumentIdentity)InstrumentIdentity;

    #region File reading
    [Obsolete($"Use {nameof(ChartFile.ReadInstrument)}.")]
    public static GHLInstrument? FromFile(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default) => ExtensionHandler.Read(path, (".chart", path => ChartFile.ReadInstrument(path, instrument, config, formatting)));

    [Obsolete($"Use {nameof(ChartFile.ReadInstrumentAsync)}.")]
    public static async Task<GHLInstrument?> FromFileAsync(string path, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, FormattingRules? formatting = default, CancellationToken cancellationToken = default) => await ExtensionHandler.ReadAsync(path, (".chart", path => ChartFile.ReadInstrumentAsync(path, instrument, config, formatting, cancellationToken)));

    [Obsolete($"Use {nameof(ChartFile.ReadInstrument)} with {nameof(Metadata.Formatting)}.")]
    public static DirectoryResult<GHLInstrument?> FromDirectory(string directory, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default) => DirectoryHandler.FromDirectory(directory, (path, formatting) => FromFile(path, instrument, config, formatting));

    [Obsolete($"Use {nameof(ChartFile.ReadInstrumentAsync)} with {nameof(Metadata.Formatting)}.")]
    public static Task<DirectoryResult<GHLInstrument?>> FromDirectoryAsync(string directory, GHLInstrumentIdentity instrument, ReadingConfiguration? config = default, CancellationToken cancellationToken = default) => DirectoryHandler.FromDirectoryAsync(directory, async (path, formatting) => await FromFileAsync(path, instrument, config, formatting, cancellationToken), cancellationToken);

    internal override InstrumentMapper<GHLChord> GetMidiMapper(WritingSession session)
    {
        throw new NotImplementedException();
    }
    #endregion
}
