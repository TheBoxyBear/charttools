﻿namespace ChartTools.IO.Configuration.Common;

public interface ICommonWritingConfiguration : ICommonConfiguration
{
    /// <see cref="Configuration.UnsupportedModifierPolicy"/>
    public UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
}
