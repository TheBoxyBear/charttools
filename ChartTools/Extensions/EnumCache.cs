namespace ChartTools.Extensions;

/// <summary>
/// Holds a cache of defined values for an enum where <see cref="Enum.GetValues{TEnum}()"/> is to be called frequently.
/// </summary>
/// <typeparam name="T">Type of enum</typeparam>
internal static class EnumCache<T> where T : struct, Enum
{
    /// <summary>
    /// Cached values
    /// </summary>
    /// <remarks>Generates the cache on first call.</remarks>
    public static T[] Values => _values ??= [.. Enum.GetValues<T>()];
    private static T[]? _values;

    /// <summary>
    /// Clears the cache.
    /// </summary>
    public static void Clear() => _values = null;
}
