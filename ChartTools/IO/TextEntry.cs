namespace ChartTools.IO;

/// <summary>
/// Line of text file data
/// </summary>
internal readonly struct TextEntry
{
	/// <summary>
	/// Text before the equal sign
	/// </summary>
	public ReadOnlyMemory<char> Key { get; }

	/// <summary>
	/// Text after the equal sign
	/// </summary>
	public ReadOnlyMemory<char> Value { get; }

	public TextEntry(in ReadOnlyMemory<char> key, in ReadOnlyMemory<char> value)
	{
		Key   = key;
		Value = value;
	}

	public TextEntry(in ReadOnlyMemory<char> line)
	{
		int separatorIndex = line.Span.IndexOf('=');

		if (separatorIndex == -1)
			throw new EntryException();

		Key   = line[0..separatorIndex].Trim();
		Value = line[(separatorIndex + 1)..].Trim();
	}
}
