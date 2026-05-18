using System.ComponentModel;

namespace ChartTools.Extensions;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class MemoryExtensions
{
#if !NETCOREAPP3_0_OR_GREATER
	/// <summary>
	/// Removes all leading and trailing white-space characters from the memory.
	/// </summary>
	public static ReadOnlyMemory<char> Trim(this ReadOnlyMemory<char> memory)
		=> memory.TrimStart().TrimEnd();

	/// <summary>
	/// Removes all leading white-space characters from the memory.
	/// </summary>
	public static ReadOnlyMemory<char> TrimStart(this ReadOnlyMemory<char> memory)
	{
		int start = 0;

		for (; start < memory.Length; start++)
			if (!char.IsWhiteSpace(memory.Span[start]))
				break;

		return memory[start..];
	}

	/// <summary>
	/// Removes all trailing white-space characters from the memory.
	/// </summary>
	public static ReadOnlyMemory<char> TrimEnd(this ReadOnlyMemory<char> memory)
	{
		int end = memory.Length - 1;

		for (; end >= 0; end--)
			if (!char.IsWhiteSpace(memory.Span[end]))
				break;

		return memory[..(end + 1)];
	}

	/// <summary>
	/// Removes all leading and trailing occurrences of a specified element from the memory.
	/// </summary>
	/// <param name="memory">The source memory from which the element is removed.</param>
	/// <param name="trimElement">The specified element to look for and remove.</param>
	public static ReadOnlyMemory<T> Trim<T>(this ReadOnlyMemory<T> memory, T trimElement)
		where T : IEquatable<T>?
	{
		ReadOnlySpan<T> span = memory.Span;

		int
			start  = ClampStart(span, trimElement),
			length = ClampEnd(span, start, trimElement);

		return memory.Slice(start, length);
	}

	/// <summary>
	/// Removes all leading occurrences of a specified element from the memory.
	/// </summary>
	/// <param name="memory">The source memory from which the element is removed.</param>
	/// <param name="trimElement">The specified element to look for and remove.</param>
	public static ReadOnlyMemory<T> TrimStart<T>(this ReadOnlyMemory<T> memory, T trimElement)
		where T : IEquatable<T>?
		=> memory[ClampStart(memory.Span, trimElement)..];

	/// <summary>
	/// Removes all trailing occurrences of a specified element from the memory.
	/// </summary>
	/// <param name="memory">The source memory from which the element is removed.</param>
	/// <param name="trimElement">The specified element to look for and remove.</param>
	public static ReadOnlyMemory<T> TrimEnd<T>(this ReadOnlyMemory<T> memory, T trimElement)
		where T : IEquatable<T>?
		=> memory[..ClampEnd(memory.Span, 0, trimElement)];

	/// <summary>
	/// Delimits all leading occurrences of a specified element from the span.
	/// </summary>
	/// <param name="span">The source span from which the element is removed.</param>
	/// <param name="trimElement">The specified element to look for and remove.</param>
	private static int ClampStart<T>(ReadOnlySpan<T> span, T trimElement)
		where T : IEquatable<T>?
	{
		int start = 0;

		if (trimElement != null)
			for (; start < span.Length; start++)
				if (!trimElement.Equals(span[start]))
					break;
		else
			for (; start < span.Length; start++)
				if (span[start] != null)
					break;

		return start;
	}

	/// <summary>
	/// Delimits all trailing occurrences of a specified element from the span.
	/// </summary>
	/// <param name="span">The source span from which the element is removed.</param>
	/// <param name="start">The start index from which to being searching.</param>
	/// <param name="trimElement">The specified element to look for and remove.</param>
	private static int ClampEnd<T>(ReadOnlySpan<T> span, int start, T trimElement)
		where T : IEquatable<T>?
	{
		// Initially, start==len==0. If ClampStart trims all, start==len
		System.Diagnostics.Debug.Assert((uint)start <= span.Length);

		int end = span.Length - 1;

		if (trimElement != null)
			for (; end >= start; end--)
				if (!trimElement.Equals(span[end]))
					break;
		else
			for (; end >= start; end--)
				if (span[end] != null)
					break;

		return end - start + 1;
	}
#endif
}
