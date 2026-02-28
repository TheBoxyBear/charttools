namespace ChartTools.IO.Configuration.Common;

/// <summary>
/// Reading options common to all file formats
/// </summary>
public interface ICommonReadingConfiguration : ICommonConfiguration
{
    /// <summary>
    /// Policy for handling unknown sections in a file
    /// </summary>
    public UnknownSectionPolicy UnknownSectionPolicy { get; }
}
