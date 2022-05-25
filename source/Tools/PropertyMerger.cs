using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Tools
{
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
        public static void Merge<T>(this T current, bool overwriteNonNull, bool deepMerge, params T[] newValues)
        {
            T? newValue = current;
            var stringType = typeof(string);
            var nullableType = typeof(Nullable);

            foreach (var prop in GetProperties(typeof(T)))
                MergeValue(current, prop, GetValues(newValues.Cast<object>(), prop));

            void MergeValue(object? source, PropertyInfo prop, IEnumerable<object> newValues)
            {
                var value = prop.GetValue(source);

                if (deepMerge && !prop.PropertyType.IsPrimitive && prop.PropertyType != stringType && Nullable.GetUnderlyingType(prop.PropertyType) is null)
                {
                    if (value is not null)
                        foreach (var deepProp in GetProperties(prop.PropertyType))
                            MergeValue(value, deepProp, GetValues(newValues, deepProp));
                }
                else if (value is null || overwriteNonNull)
                {
                    var newVal = newValues.FirstOrDefault(newVal => newVal is not null);

                    if (newVal is not null)
                        prop.SetValue(source, newVal);
                }
            }

            IEnumerable<PropertyInfo> GetProperties(Type type) => type.GetProperties().Where(i => i.CanWrite);
            IEnumerable<object> GetValues(IEnumerable<object> sources, PropertyInfo prop) => sources.Select(s => prop.GetValue(s)).NonNull();
        }
    }
}
