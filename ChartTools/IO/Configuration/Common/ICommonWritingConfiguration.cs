namespace ChartTools.IO.Configuration.Common;

public interface ICommonWritingConfiguration
{
    /// <see cref="Configuration.UnsupportedModifierPolicy"/>
    public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
}
