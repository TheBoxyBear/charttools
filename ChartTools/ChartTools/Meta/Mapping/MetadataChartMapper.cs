using System.Diagnostics.CodeAnalysis;

using ChartTools.IO;
using ChartTools.IO.Chart;

namespace ChartTools.Meta.Mapping;

internal sealed partial class MetadataChartMapper : MetadataMapper
{
	public static MetadataChartMapper Shared { get; } = new();

	public override FileType FileType => FileType.Chart;

	public override string? Get(Metadata metadata, in ReadOnlySpan<char> key)
	{
		ValidateKey(in key);

		return TryGetFromAttribute(metadata, key, out var value)
			? value : key switch
			{
				ChartFormatting.Year => metadata.Year is null ? null : $"\", {metadata.Year}\"",
				ChartFormatting.AudioOffset => metadata.AudioOffset?.TotalSeconds.ToString(),
				_ => FindUndentified(metadata, in key)
			};
	}

	public override void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		ValidateKey(in key);

		if (!TrySetFromAttribute(metadata, in key, in value))
			switch (key)
			{
				case ChartFormatting.Year:
					metadata.Year = ValueParser.Parse<ushort>(value.Trim('"').TrimStart(','), nameof(Metadata.Year));
					break;
				case ChartFormatting.AudioOffset:
					metadata.AudioOffset = TimeSpan.FromSeconds(ValueParser.Parse<float>(value, nameof(Metadata.AudioOffset)));
					break;
				default:
					AddUnidentified(metadata, in key, in value);
					break;
			}
	}

	public override void Remove(Metadata metadata, in ReadOnlySpan<char> key)
	{
		ValidateKey(in key);

		if (!TryRemoveFromAttribute(metadata, in key))
			RemoveUnidentified(metadata, in key);
	}

	public override bool Contains(Metadata metadata, in ReadOnlySpan<char> key)
		=> TryContainsFromAttribute(metadata, in key) ?? key switch
		{
			ChartFormatting.Year        => metadata.Year is not null,
			ChartFormatting.AudioOffset => metadata.AudioOffset is not null,
			_ => ContainsUnidentified(metadata, in key)
		};

	public override IEnumerable<TextEntry> GetAll(Metadata metadata)
		=> GetAllFromAttributes(metadata).Concat(GetAllUnidentified(metadata));

	private static partial bool TryGetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value);

	private static partial bool TrySetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	private static partial bool TryRemoveFromAttribute(Metadata metadata, in ReadOnlySpan<char> key);

	private static partial bool? TryContainsFromAttribute(Metadata metadata, in ReadOnlySpan<char> key);

	private partial IEnumerable<TextEntry> GetAllFromAttributes(Metadata metadata);

	private MetadataChartMapper() { }
}
