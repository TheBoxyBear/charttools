using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.Extensions;

public static class TypePassthrough
{
    public static T? Passthrough<T>(this Action action)
    {
        action();
        return default;
    }
}
