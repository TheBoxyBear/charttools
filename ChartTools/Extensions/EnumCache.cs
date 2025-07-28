namespace ChartTools.Extensions;

internal static class EnumCache<T> where T : struct, Enum
{
	private static T[]? m_values;
	public static T[] Values => m_values ??= [.. Enum.GetValues<T>()];

	public static void Clear() => m_values = null;
}
