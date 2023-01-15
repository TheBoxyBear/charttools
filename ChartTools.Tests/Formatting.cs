namespace ChartTools.Tests;

public static class Formatting
{
    public static string FormatCollection<T>(IEnumerable<T> items) => string.Join(' ', items);
}
