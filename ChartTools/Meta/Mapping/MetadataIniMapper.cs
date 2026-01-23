using ChartTools.IO;
using ChartTools.IO.Ini;

using System.Diagnostics.CodeAnalysis;

namespace ChartTools.Meta.Mapping;

internal partial class MetadataIniMapper : MetadataMapper
{
	public static MetadataIniMapper Shared { get; } = new();

	public override FileType FileType => FileType.Ini;

	public override string? Get(Metadata metadata, in ReadOnlySpan<char> key)
		=> TryGetFromAttribute(metadata, key, out var value)
			? value : key switch
			{
				IniFormatting.AudioOffset => metadata.AudioOffset?.TotalMilliseconds.ToString(),
				IniFormatting.VideoOffset => metadata.VideoOffset?.TotalMilliseconds.ToString(),
				IniFormatting.Modchart => metadata.IsModchart.HasValue ? (metadata.IsModchart.Value ? "1" : "0") : null,
				_ => FindUndentified(metadata, key)
			};

	public override void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		if (TrySetFromAttribute(metadata, in key, in value))
			return;
	}

	public override void Remove(Metadata metadata, in ReadOnlySpan<char> key)
	{
		if (TryRemoveFromAttribute(metadata, in key))
			return;
	}

	public override IEnumerable<TextEntry> GetAll()
	{
		throw new NotImplementedException();
	}

	private static partial bool TryGetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value);

	private static partial bool TrySetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	private static partial bool TryRemoveFromAttribute(Metadata metadata, in ReadOnlySpan<char> key);

	private static partial IEnumerable<TextEntry> GetAllFromAttributes(Metadata metadata);

	private MetadataIniMapper() { }
}
