using ChartTools.IO;

namespace ChartTools.Meta.Mapping;

internal abstract class MetadataMapper
{
	public abstract FileType FileType { get; }

	public abstract string? Get(Metadata metadata, in ReadOnlySpan<char> key);

	public abstract void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	public abstract void Remove(Metadata metadata, in ReadOnlySpan<char> key);

	public abstract IEnumerable<TextEntry> GetAll();

	protected string? FindUndentified(Metadata metadata, in ReadOnlySpan<char> key)
		=> metadata.UnidentifiedData.TryGetValue(new()
		{
			Key    = key.ToString(),
			Origin = FileType
		}, out UnidentifiedMetadata found)
			? found.Value
			: null;
}
