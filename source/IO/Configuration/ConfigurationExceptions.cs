using System;

namespace ChartTools.IO.Configuration
{
    internal static class ConfigurationExceptions
    {
        public static ArgumentException UnsupportedPolicy(Enum policy) => new("Policy is not supported.", $"{policy}");
    }
}
