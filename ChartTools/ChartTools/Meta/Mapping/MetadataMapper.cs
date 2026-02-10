using ChartTools.IO;

using System.Diagnostics.CodeAnalysis;

namespace ChartTools.Meta.Mapping;

internal abstract class MetadataMapper
{
	public abstract FileType FileType { get; }

	public abstract string? Get(Metadata metadata, in ReadOnlySpan<char> key);

	public abstract void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	public abstract void Remove(Metadata metadata, in ReadOnlySpan<char> key);

	public abstract bool Contains(Metadata metadata, in ReadOnlySpan<char> key);

	public abstract IEnumerable<TextEntry> GetAll(Metadata metadata);

	public bool TryGet(Metadata metadata, in ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value)
	{
		value = Get(metadata, in key);
		return value is not null;
	}

	protected string? FindUndentified(Metadata metadata, in ReadOnlySpan<char> key)
	{
		foreach (UnidentifiedMetadata data in metadata.UnidentifiedData)
			if (data.Origin == FileType && data.Key == key.ToString())
				return data.Value;

		return null;
	}

	protected void AddUnidentified(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
		=> metadata.UnidentifiedData.Add(new()
		{
			Key    = key.ToString(),
			Value  = value.ToString(),
			Origin = FileType
		});

	protected void RemoveUnidentified(Metadata metadata, in ReadOnlySpan<char> key)
		=> metadata.UnidentifiedData.Remove(new()
		{
			Key    = key.ToString(),
			Origin = FileType
		});

	protected bool ContainsUnidentified(Metadata metadata, in ReadOnlySpan<char> key)
		=> metadata.UnidentifiedData.Contains(new()
		{
			Key = key.ToString(),
			Origin = FileType
		});

	protected IEnumerable<TextEntry> GetAllUnidentified(Metadata metadata)
		=> metadata.UnidentifiedData
			.Where(data => data.Origin == FileType)
			.Select(data => new TextEntry(data.Key.AsMemory(), data.Value.AsMemory()));

	protected static void ValidateKey(in ReadOnlySpan<char> key)
	{
		if (key.IsEmpty)
			throw new ArgumentException("Key cannot be empty.", nameof(key));
	}
}
