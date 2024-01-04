namespace ChartTools.IO.Configuration.Common;

public interface ICommonReadingConfiguration
{
    /// <inheritdoc cref="Configuration.UnknownSectionPolicy"/>
    public UnknownSectionPolicy UnknownSectionPolicy { get; }
}
