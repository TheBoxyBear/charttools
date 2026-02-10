using System.Collections.Immutable;

namespace ChartTools.Extensions.Enums;

/// <summary>
/// Holds a cache of defined values for an enum where <see cref="Enum.GetValues{TEnum}()"/> is to be called frequently.
/// </summary>
/// <typeparam name="T">Type of enum</typeparam>
public static class EnumCache<T> where T : struct, Enum
{
	/// <summary>
	/// Cached values
	/// </summary>
	/// <remarks>Generates the cache on first call.</remarks>
	public static ImmutableArray<T> Values => s_values ??=
#if NET5_0_OR_GREATER
		[.. Enum.GetValues<T>()];
#else
		[.. Enum.GetValues(typeof(T)).Cast<T>()];
#endif

	private static ImmutableArray<T>? s_values;

	public static TypeCode UnderlyingTypeCode
		=> s_typeCode ??= Type.GetTypeCode(Enum.GetUnderlyingType(typeof(T)));

	private static TypeCode? s_typeCode;

	/// <summary>
	/// Clears the cache.
	/// </summary>
	public static void Clear()
		=> s_values = null;
}
