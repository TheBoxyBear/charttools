namespace ChartTools.IO.Configuration.Common;

public interface ICommonWritingConfiguration
{
    /// <see cref="Configuration.UnsupportedModifiersPolicy"/>
    public UnsupportedModifiersPolicy UnsupportedModifiersPolicy { get; init; }
}
