﻿using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Common;

namespace ChartTools.IO.Chart.Configuration;

public record ChartWritingConfiguration : CommonChartConfiguration, ICommonWritingConfiguration
{
    public required UnsupportedModifierPolicy UnsupportedModifierPolicy { get; init; }
}
