namespace ChartTools.IO.Configuration.Common;

public interface ICommonReadingConfiguration : ICommonConfiguration
{
    public UnknownSectionPolicy UnknownSectionPolicy { get; }
}
