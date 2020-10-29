using System.Reflection;

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
        internal static void Merge<T>(this T current, bool overwriteNonNull, params T[] newValues)
        {
            foreach (PropertyInfo i in typeof(T).GetProperties())
                if (i.GetValue(current) is null || overwriteNonNull)
                    foreach (T newValue in newValues)
                    {
                        object newProperty = i.GetValue(newValues);

                        if (newProperty is not null)
                            i.SetValue(current, newProperty);
                    }
        }
    }
}
