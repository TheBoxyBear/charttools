using System.Reflection;
using ChartTools.Extensions.Linq;

namespace ChartTools.Tools;

/// <summary>
/// Provides methods to merge properties between two instances
/// </summary>
public static class PropertyMerger
{
	/// <summary>
	/// Replaces the property values of an instance with the first non-null equivalent from other instances.
	/// </summary>
	/// <remarks>If overwriteNonNull is <see langword="false"/>, only replaces property values that are null in the original instance.</remarks>
	/// <param name="current">Item to assign the property values to</param>
	/// <param name="overwriteNonNull">If <see langword="false"/>, only replaces property values that are null in the original instance.</param>
	/// <param name="newValues">Items to pull new property values from in order of priority</param>
	public static void Merge<T>(this T current, bool overwriteNonNull, bool deepMerge, params IEnumerable<T> newValues)
	{
		T? newValue = current;

		Type
			stringType   = typeof(string),
			nullableType = typeof(Nullable);

		foreach (PropertyInfo prop in GetProperties(typeof(T)))
			MergeValue(current, prop, GetValues(newValues.Cast<object>(), prop));

		void MergeValue(object? source, PropertyInfo prop, IEnumerable<object> newValues)
		{
			object? value = prop.GetValue(source);

			if (deepMerge && !prop.PropertyType.IsPrimitive && prop.PropertyType != stringType && Nullable.GetUnderlyingType(prop.PropertyType) is null)
			{
				if (value is not null)
					foreach (PropertyInfo deepProp in GetProperties(prop.PropertyType))
						MergeValue(value, deepProp, GetValues(newValues, deepProp));
			}
			else if (value is null || overwriteNonNull)
			{
				object? newVal = newValues.FirstOrDefault(newVal => newVal is not null);

				if (newVal is not null)
					prop.SetValue(source, newVal);
			}
		}

		IEnumerable<PropertyInfo> GetProperties(Type type) => type.GetProperties().Where(i => i.CanWrite);
		IEnumerable<object> GetValues(IEnumerable<object> sources, PropertyInfo prop) => sources.Select(prop.GetValue).NonNull();
	}
}
