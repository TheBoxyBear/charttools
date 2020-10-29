using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// Provides additionnal methods to Enum
    /// </summary>
    internal static class EnumExtensions
    {
        /// <summary>
        /// Gets all values of an <see langword="enum"/>.
        /// </summary>
        /// <exception cref="ArgumentException"/>
        public static IEnumerable<TEnum> GetValues<TEnum>() where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
                throw new ArgumentException("Type is not an enum.");
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        }
    }

    internal class ActivatorExtensions
    {
        /// <inheritdoc cref="Activator.CreateInstance(Type, bool)"/>
        internal static T CreateInstance<T>(bool nonPublic) => (T)Activator.CreateInstance(typeof(T), nonPublic);
        /// <inheritdoc cref="Activator.CreateInstance(Type, object[])"/>
        internal static T CreateInstance<T>(params object[] args)
        {
            object instance;

            try { instance = Activator.CreateInstance(typeof(T), args); }
            catch (Exception e) { throw e; }

            return (T)instance;
        }
    }
}
namespace System.Linq
{
    /// <summary>
    /// Provides addtionnal methods to Linq
    /// </summary>
    internal static class LinqExtensions
    {
        /// <summary>
        /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// </summary>
        public static T FirstOrDefault<T>(this IEnumerable<T> items, Predicate<T> predicate, T defaultValue)
        {
            foreach (T item in items)
                if (predicate(item))
                    return item;
            return defaultValue;
        }
        /// <summary>
        /// <inheritdoc cref="Enumerable.FirstOrDefault{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
        /// </summary>
        public static T FirstOrDefault<T>(this IEnumerable<T> items, Predicate<T> predicate, T defaultValue, out bool returnedDefault)
        {
            foreach (T item in items)
                if (predicate(item))
                {
                    returnedDefault = false;
                    return item;
                }

            returnedDefault = true;
            return defaultValue;
        }
    }
}
