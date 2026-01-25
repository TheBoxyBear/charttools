using System.Diagnostics.CodeAnalysis;

using ChartTools.IO;
using ChartTools.IO.Chart;

namespace ChartTools.Meta.Mapping;

internal sealed partial class MetadataChartMapper : MetadataMapper
{
	public static MetadataChartMapper Shared { get; } = new();

	public override FileType FileType => FileType.Chart;

	public override string? Get(Metadata metadata, in ReadOnlySpan<char> key)
		=> TryGetFromAttribute(metadata, key, out var value)
			? value : key switch
			{
				ChartFormatting.Year        => metadata.Year is null ? null : $"\", {metadata.Year}\"",
				ChartFormatting.AudioOffset => metadata.AudioOffset?.TotalMilliseconds.ToString(),
				_ => FindUndentified(metadata, in key)
			};

	public override void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		if (TrySetFromAttribute(metadata, in key, in value))
			return;

		switch (key)
		{
			case ChartFormatting.Year:
				metadata.Year = ValueParser.Parse<ushort>(value.TrimStart(','), "year");
				break;
			case ChartFormatting.AudioOffset:
				metadata.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<float>(value, "audio offset") * 1000);
				break;
			default:
				AddUnidentified(metadata, in key, in value);
				break;
		}
	}

	public override void Remove(Metadata metadata, in ReadOnlySpan<char> key)
	{
		if (!TryRemoveFromAttribute(metadata, in key))
			RemoveUnidentified(metadata, in key);
	}

	public override IEnumerable<TextEntry> GetAll(Metadata metadata)
	{
		foreach (TextEntry entry in GetAllFromAttributes(metadata))
			yield return entry;

		if (metadata.Year is not null)
			yield return new(ChartFormatting.Year, Get(metadata, ChartFormatting.Year)!);

		foreach (TextEntry entry in GetAllUnidentified(metadata))
			yield return entry;
	}

	private static partial bool TryGetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value);

	private static partial bool TrySetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	private static partial bool TryRemoveFromAttribute(Metadata metadata, in ReadOnlySpan<char> key);

	private static partial IEnumerable<TextEntry> GetAllFromAttributes(Metadata metadata);

	private MetadataChartMapper() { }
}
