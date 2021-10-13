using System.Reflection;
using System.Linq;

namespace ChartTools.InternalTools
{
    /// <summary>
    /// Provides methods to merge properties between two instances
    /// </summary>
    internal static class ProperyMerger
    {
        /// <summary>
        /// Replaces the property values of an instance with the first non-null equivalent from other instances.
        /// </summary>
        /// <remarks>If overwriteNonNull is <see langword="false"/>, only replaces property values that are null in the original instance.</remarks>
        /// <param name="current">Item to assign the property values to</param>
        /// <param name="overwriteNonNull">If <see langword="false"/>, only replaces property values that are null in the original instance.</param>
        /// <param name="newValues">Items to pull new property values from in order of priority</param>
        internal static void Merge<T>(this T current, bool overwriteNonNull, params T[] newValues)
        {
            foreach (PropertyInfo i in typeof(T).GetProperties())
                if (i.GetValue(current) is null || overwriteNonNull)
                    foreach (object newProperty in from newValue in newValues
                                                   let newProperty = i.GetValue(newValues)
                                                   where newProperty is not null
                                                   select newProperty)
                        i.SetValue(current, newProperty);
        }
    }
}
