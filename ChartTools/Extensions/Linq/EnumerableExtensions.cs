using ChartTools.Extensions.Collections.Alternating;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace ChartTools.Extensions.Linq;

/// <summary>
/// Provides a set of extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
	#region First
	/// <summary>
	/// Gets the first item that meets a condition from a collection or a default value if no such item was found.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Collection to get the first item of</param>
	/// <param name="predicate">Condition to compare items against</param>
	/// <param name="defaultValue">Value to return if no items meeting the condition were found</param>"
	/// <param name="returnedDefault"><see langword="true"/> if no items meeting the condition were found</param>
	public static T? FirstOrDefault<T>(this IEnumerable<T> source, Predicate<T> predicate, T? defaultValue, out bool returnedDefault)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		foreach (T item in source)
			if (predicate(item))
			{
				returnedDefault = false;
				return item;
			}

		returnedDefault = true;
		return defaultValue;
	}

	/// <summary>
	/// Tries to get the first item that meets a condition from a collection.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Collection to get the first item of</param>
	/// <param name="predicate">Method that returns <see langword="true"/> if a given item meets the condition</param>
	/// <param name="item">Found item</param>
	/// <returns><see langword="true"/> if an item was found</returns>
	public static bool TryGetFirst<T>(this IEnumerable<T> source, Predicate<T> predicate, [MaybeNullWhen(false)] out T? item)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		foreach (T t in source)
			if (predicate(t))
			{
				item = t;
				return true;
			}

		item = default;
		return false;
	}

	/// <summary>
	/// Tries to get the first element of a collection.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Source of items</param>
	/// <param name="result">Found item</param>
	/// <returns><see langword="true"/> if an item was found</returns>
	public static bool TryGetFirst<T>(this IEnumerable<T> source, [MaybeNullWhen(false)] out T result)
	{
		using IEnumerator<T> enumerator = source.GetEnumerator();
		bool success = enumerator.MoveNext();

		result = success ? enumerator.Current : default;
		return success;
	}

	/// <summary>
	/// Tries to get the first item of a given type in a collection.
	/// </summary>
	/// <typeparam name="TResult">Type to get items of</typeparam>
	/// <param name="source">Source of items</param>
	/// <param name="result">Found item</param>
	/// <returns><see langword="true"/> if an item was found</returns>
	public static bool TryGetFirstOfType<TResult>(this IEnumerable source, [MaybeNullWhen(false)] out TResult result)
		=> source.OfType<TResult>().TryGetFirst(out result);
	#endregion

	/// <summary>
	/// Excludes <see langword="null"/> items from a set of references.
	/// </summary>
	/// <typeparam name="T">Type of items of references types or boxed values</typeparam>
	public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> source)
		=> source.Where(t => t is not null)!;

	/// <summary>
	/// Excludes <see langword="null"/> item from a set of nullable values.
	/// </summary>
	/// <typeparam name="T">Underlying value type</typeparam>
	/// <param name="source">Set of values wrapped in <see cref="Nullable{T}"/></param>
	/// <returns>Set of the nullable values unwrapped to the underlying type with <see langword="null"/> items excluded.</returns>
	public static IEnumerable<T> NonNull<T>(this IEnumerable<T?> source) where T : struct
	{
		foreach (T? item in source)
			if (item is not null)
				yield return item.Value;
	}

	#region Replace
	/// <summary>
	/// Replaces items that meet a condition with another item.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">The collection to replace the items of</param>
	/// <param name="predicate">A function that determines if an item must be replaced</param>
	/// <param name="replacement">The item to replace items with</param>
	public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, Predicate<T> predicate, T replacement)
	{
		ArgumentNullException.ThrowIfNull(predicate);

		foreach (T item in source)
			yield return predicate(item) ? replacement : item;
	}

	/// <summary>
	/// Replaces a section from a collection with other items.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Items to replace a section in</param>
	/// <param name="replacement">Set of rules defining the replacement</param>
	/// <returns>Items with the specified section replaced</returns>
	/// <remarks>Items that match <see cref="SectionReplacement{T}.StartReplace"/> or <see cref="SectionReplacement{T}.EndReplace"/> are not included in the output.</remarks>
	public static IEnumerable<T> ReplaceSection<T>(this IEnumerable<T> source, SectionReplacement<T> replacement)
	{
		if (replacement.StartReplace is null)
			throw new NullReferenceException(nameof(replacement.StartReplace));
		if (replacement.EndReplace is null)
			throw new NullReferenceException(nameof(replacement.EndReplace));

		IEnumerator<T> itemsEnumerator = source.GetEnumerator();

		// Initialize the enumerator
		if (!itemsEnumerator.MoveNext())
		{
			// Return the replacement
			if (replacement.AddIfMissing)
				foreach (T item in replacement.Replacement)
					yield return item;

			yield break;
		}

		// Return original until startReplace
		while (!replacement.StartReplace(itemsEnumerator.Current))
		{
			yield return itemsEnumerator.Current;

			if (!itemsEnumerator.MoveNext())
			{
				// Return the replacement
				if (replacement.AddIfMissing)
					foreach (T item in replacement.Replacement)
						yield return item;
				yield break;
			}
		}

		// Return replacement
		foreach (T item in replacement.Replacement)
			yield return item;

		// Find the end of the section to replace
		do
			if (!itemsEnumerator.MoveNext())
				yield break;
		while (replacement.EndReplace(itemsEnumerator.Current));

		// Return the rest
		while (itemsEnumerator.MoveNext())
			yield return itemsEnumerator.Current;
	}

	/// <summary>
	/// Replaces multiple sections of items from a collection.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Items to replace sections in</param>
	/// <param name="replacements">Set of definitions of section replacements</param>
	/// <returns>Items with the specified section replaced</returns>
	/// <remarks>Items that match <see cref="SectionReplacement{T}.StartReplace"/> or <see cref="SectionReplacement{T}.EndReplace"/> are not included in the output.</remarks>
	public static IEnumerable<T> ReplaceSections<T>(this IEnumerable<T> source, IEnumerable<SectionReplacement<T>> replacements)
	{
		if (replacements is null || !replacements.Any())
		{
			foreach (T item in source)
				yield return item;
			yield break;
		}

		List<SectionReplacement<T>> replacementList = [.. replacements];
		using IEnumerator<T> itemsEnumerator = source.GetEnumerator();

		if (!itemsEnumerator.MoveNext())
		{
			foreach (T item in AddMissing())
				yield return item;

			yield break;
		}

		do
		{
			// Find a matching replacement start
			if (replacementList.TryGetFirst(r => r.StartReplace(itemsEnumerator.Current), out SectionReplacement<T> replacement))
			{
				// Move to the end of the section to replace
				do
					if (!itemsEnumerator.MoveNext())
					{
						foreach (T item in AddMissing())
							yield return item;
						yield break;
					}
				while (!replacement.EndReplace(itemsEnumerator.Current));

				// Return the replacement
				foreach (T item in replacement.Replacement)
					yield return item;

				replacementList.Remove(replacement);
			}
			else
			{
				yield return itemsEnumerator.Current;

				if (!itemsEnumerator.MoveNext())
				{
					foreach (T item in AddMissing())
						yield return item;
					yield break;
				}
			}
		}
		// Continue until all replacements are applied
		while (replacementList.Count > 0);

		// Return the rest of the items
		while (itemsEnumerator.MoveNext())
			yield return itemsEnumerator.Current;

		IEnumerable<T> AddMissing()
		{
			// Return remaining replacements
			foreach (SectionReplacement<T> replacement in replacementList.Where(r => r.AddIfMissing))
				// Return the replacement
				foreach (T item in replacement.Replacement)
					yield return item;
		}
	}

	/// <summary>
	/// Removes a section of items from a collection.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Source items to remove a section of</param>
	/// <param name="startRemove">Function that determines the start of the section to replace</param>
	/// <param name="endRemove">Function that determines the end of the section to replace</param>
	/// <returns>Items with the specified section removed</returns>
	/// <remarks>Items that match <paramref name="startRemove"/> or <paramref name="endRemove"/> are not included in the output.</remarks>
	public static IEnumerable<T> RemoveSection<T>(this IEnumerable<T> source, Predicate<T> startRemove, Predicate<T> endRemove)
	{
		IEnumerator<T> itemsEnumerator = source.GetEnumerator();

		// Initialize the enumerator
		if (!itemsEnumerator.MoveNext())
			yield break;

		// Move to the start of items to remove
		while (!startRemove(itemsEnumerator.Current))
			if (!itemsEnumerator.MoveNext())
				yield break;

		// Skip items to remove
		do
			if (!itemsEnumerator.MoveNext())
				yield break;
		while (!endRemove(itemsEnumerator.Current));

		// Return the rest
		while (itemsEnumerator.MoveNext())
			yield return itemsEnumerator.Current;
	}
	#endregion

	/// <summary>
	/// Loops through a set of objects and returns a set of tuples containing the current object and the previous one.
	/// </summary>
	/// <param name="source">Items to loop through</param>
	/// <param name="firstPrevious">Value of the previous item in the first call of the action</param>
	public static IEnumerable<(T? previous, T current)> RelativeLoop<T>(this IEnumerable<T> source, T? firstPrevious = default)
	{
		T? previousItem = firstPrevious;

		foreach (T item in source)
		{
			yield return (previousItem, item);
			previousItem = item;
		}
	}

	public static IEnumerable<(T previous, T current)> RelativeLoopSkipFirst<T>(this IEnumerable<T> source)
	{
		using IEnumerator<T> enumerator = source.GetEnumerator();

		if (enumerator.MoveNext())
			yield break;

		T previous = enumerator.Current;

		while (enumerator.MoveNext())
			yield return (previous, enumerator.Current);
	}

	#region Unique
	/// <summary>
	/// Returns distinct elements of a collection using a method to determine the equality of elements.
	/// </summary>
	/// <typeparam name="T">Type of items in the collection</typeparam>
	/// <param name="source">Collection to get distinct items from</param>
	/// <param name="comparison">Method determining if two elements are the same</param>
	public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, EqualityComparison<T?> comparison)
		=> source.Distinct(new FuncEqualityComparer<T>(comparison));

	public static bool Unique<T>(this IEnumerable<T> source)
		=> UniqueFromDistinct(source.Distinct());

	public static bool UniqueBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
		=> UniqueFromDistinct(source.DistinctBy(selector));

	private static bool UniqueFromDistinct<T>(IEnumerable<T> distinct)
		=> !distinct.Skip(1).Any();
	#endregion

	#region MinMax
	/// <summary>
	/// Finds the items for which a function returns the smallest or greatest value based on a comparison.
	/// </summary>
	/// <param name="source">Items to find the minimum or maximum of</param>
	/// <param name="selector">Function that gets the key to use in the comparison from an item</param>
	/// <param name="comparison">Function that returns <see langword="true"/> if the second item defeats the first</param>
	private static IEnumerable<T> ManyMinMaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector, Func<TKey, TKey, bool> comparison)
		where TKey : IComparable<TKey>
	{
		TKey minMaxKey;

		using (IEnumerator<T> enumerator = source.GetEnumerator())
		{
			if (!enumerator.MoveNext())
				throw new ArgumentException("The enumerable has no items.", nameof(source));

			minMaxKey = selector(enumerator.Current);

			while (enumerator.MoveNext())
			{
				TKey key = selector(enumerator.Current);

				if (comparison(key, minMaxKey))
					minMaxKey = key;
			}
		}

		return source.Where(t => selector(t).CompareTo(minMaxKey) == 0);
	}

	/// <summary>
	/// Finds the items for which a function returns the smallest value.
	/// </summary>
	/// <param name="source">Items to find the minimum or maximum of</param>
	/// <param name="selector">Function that gets the key to use in the comparison from an item</param>
	public static IEnumerable<T> ManyMinBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
		where TKey : IComparable<TKey>
		=> ManyMinMaxBy(source, selector, (key, mmkey) => key.CompareTo(mmkey) < 0);

	/// <summary>
	/// Finds the items for which a function returns the greatest value.
	/// </summary>
	/// <param name="source">Items to find the minimum or maximum of</param>
	/// <param name="selector">Function that gets the key to use in the comparison from an item</param>
	public static IEnumerable<T> ManyMaxBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
		where TKey : IComparable<TKey>
		=> ManyMinMaxBy(source, selector, (key, mmkey) => key.CompareTo(mmkey) > 0);
	#endregion

	public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
	{
		foreach (var item in source)
			yield return await Task.FromResult(item).ConfigureAwait(false);
	}

	#region Collections
	/// <summary>
	/// Combines enumerables by alternating between each source for every item.
	/// </summary>
	/// <typeparam name="T">Type of root items</typeparam>
	/// <param name="sources">Set of enumerables to alternate between</param>
	/// <returns>Combined items from all enumerables, taking one item from each before looping.</returns>
	/// <remarks>When the end of an enumerable is reached, alternating continues while skipping that enumerable until all are finished.</remarks>
	public static IEnumerable<T> Alternate<T>(this IEnumerable<IEnumerable<T>> sources)
		=> new SerialAlternatingEnumerable<T>([.. sources]);

	/// <summary>
	/// Combines enumerables by alternating between each source for every item based on a key.
	/// </summary>
	/// <typeparam name="T">Type of root items</typeparam>
	/// <typeparam name="TKey">Type of key used to compare items when alternating</typeparam>
	/// <param name="sources"><inheritdoc cref="Alternate{T}(IEnumerable{IEnumerable{T}})" path="/param[@name='sources']"/></param>
	/// <param name="selector">Selector function returning the alternate key from an item</param>
	/// <returns>Combined items from all enumerables, taking the next item with the smallest key from each enumerable.</returns>
	/// <inheritdoc cref="Alternate{T}(IEnumerable{IEnumerable{T}})" path="/remarks"/>
	public static IEnumerable<T> AlternateBy<T, TKey>(this IEnumerable<IEnumerable<T>> sources, Func<T, TKey> selector)
		where TKey : IComparable<TKey>
		=> new OrderedAlternatingEnumerable<T, TKey>(selector, [.. sources]);
	#endregion
}
