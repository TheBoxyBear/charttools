using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections;
using ChartTools.SystemExtensions.Linq;

namespace ChartTools.Internal
{
    /// <summary>
    /// Provides methods to merge properties between two instances
    /// </summary>
    internal static class PropertyMerger
    {
        /// <summary>
        /// Replaces the property values of an instance with the first non-null equivalent from other instances.
        /// </summary>
        /// <remarks>If overwriteNonNull is <see langword="false"/>, only replaces property values that are null in the original instance.</remarks>
        /// <param name="current">Item to assign the property values to</param>
        /// <param name="overwriteNonNull">If <see langword="false"/>, only replaces property values that are null in the original instance.</param>
        /// <param name="newValues">Items to pull new property values from in order of priority</param>
        internal static void Merge<T>(T current, bool overwriteNonNull, bool deepMerge, params T[] newValues)
        {
            T? newValue = current;
            var stringType = typeof(string);

            foreach (var prop in typeof(T).GetProperties())
                MergeValue(current, prop, GetValues(newValues.Cast<object>(), prop));

            void MergeValue(object? source, PropertyInfo prop, IEnumerable<object> newValues)
            {
                var value = prop.GetValue(source);

                if (value is null || overwriteNonNull)
                {
                    if (deepMerge && prop.PropertyType.IsPrimitive && prop.PropertyType != stringType)
                        foreach (var deepProp in prop.PropertyType.GetProperties())
                            MergeValue(deepProp.GetValue(value), deepProp,GetValues(newValues, deepProp));
                    else
                    {
                        var newVal = newValues.FirstOrDefault(newVal => newVal is not null);

                        if (newVal is not null)
                            prop.SetValue(source, newVal);
                    }
                }
            }

            IEnumerable<object> GetValues(IEnumerable<object> sources, PropertyInfo prop) => sources.Select(s => prop.GetValue(s)).NonNull();
        }
    }
}
