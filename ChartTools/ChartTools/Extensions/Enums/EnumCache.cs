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
	public static ImmutableArray<T> Values
	{
		get
		{
			if (field.Length == 0)
				field = [.. Enum.GetValues<T>()];

			return field;
		}
		private set;
	}

	/// <summary>
	/// Clears the cache.
	/// </summary>
	public static void Clear()
		=> Values = [];
}
