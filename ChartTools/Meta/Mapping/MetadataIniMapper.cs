using ChartTools.IO;
using ChartTools.IO.Formatting;
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
		if (!TrySetFromAttribute(metadata, in key, in value))
			AddUnidentified(metadata, in key, in value);
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

		if (metadata.AlbumTrack is not null)
		{
			if (metadata.Formatting.AlbumTrackKey.HasFlag(AlbumTrackKey.Track))
				yield return new(IniFormatting.Track, metadata.AlbumTrack.ToString()!);

			if (metadata.Formatting.AlbumTrackKey.HasFlag(AlbumTrackKey.AlbumTrack))
				yield return new(IniFormatting.AlbumTrack, metadata.AlbumTrack.ToString()!);
		}

		if (metadata.Charter.Name is not null)
		{
			if (metadata.Formatting.CharterKey.HasFlag(CharterKey.Charter))
				yield return new(IniFormatting.Charter, metadata.Charter.Name);

			if (metadata.Formatting.CharterKey.HasFlag(CharterKey.Frets))
				yield return new(IniFormatting.Frets, metadata.Charter.Name.ToString());
		}

		foreach (TextEntry entry in GetAllUnidentified(metadata))
			yield return entry;
	}

	private static partial bool TryGetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, [MaybeNullWhen(false)] out string value);

	private static partial bool TrySetFromAttribute(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value);

	private static partial bool TryRemoveFromAttribute(Metadata metadata, in ReadOnlySpan<char> key);

	private partial IEnumerable<TextEntry> GetAllFromAttributes(Metadata metadata);

	private MetadataIniMapper() { }
}
