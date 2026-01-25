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
				IniFormatting.Modchart    => metadata.IsModchart.HasValue ? (metadata.IsModchart.Value ? "1" : "0") : null,
				IniFormatting.Track   or IniFormatting.AlbumTrack => metadata.AlbumTrack?.ToString(),
				IniFormatting.Charter or IniFormatting.Frets      => metadata.Charter.Name,
				_ => FindUndentified(metadata, key)
			};

	public override void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		if (!TrySetFromAttribute(metadata, in key, in value))
			switch (key)
			{
				case IniFormatting.AlbumTrack:
					metadata.AlbumTrack = ValueParser.Parse<byte>(in value, nameof(Metadata.AlbumTrack));
					metadata.Formatting.AlbumTrackKey |= AlbumTrackKeys.AlbumTrack;
					break;
				case IniFormatting.Track:
					metadata.AlbumTrack = ValueParser.Parse<byte>(in value, nameof(Metadata.AlbumTrack));
					metadata.Formatting.AlbumTrackKey |= AlbumTrackKeys.Track;
					break;
				case IniFormatting.Charter:
					metadata.Charter.Name = value.ToString();
					metadata.Formatting.CharterKey |= CharterKeys.Charter;
					break;
				case IniFormatting.Frets:
					metadata.Charter.Name = value.ToString();
					metadata.Formatting.CharterKey |= CharterKeys.Frets;
					break;
				default:
					AddUnidentified(metadata, in key, in value);
					break;
			}
	}

	public override void Remove(Metadata metadata, in ReadOnlySpan<char> key)
	{
		if (!TryRemoveFromAttribute(metadata, in key));
			switch (key)
			{
				case IniFormatting.AlbumTrack:
					metadata.Formatting.AlbumTrackKey &= ~AlbumTrackKeys.AlbumTrack;
					break;
				case IniFormatting.Track:
					metadata.Formatting.AlbumTrackKey &= ~AlbumTrackKeys.Track;
					break;
				case IniFormatting.Charter:
					metadata.Formatting.CharterKey &= ~CharterKeys.Charter;
					break;
				case IniFormatting.Frets:
					metadata.Formatting.CharterKey &= ~CharterKeys.Frets;
					break;
				default:
					RemoveUnidentified(metadata, in key);
					break;
			}
	}

	public override IEnumerable<TextEntry> GetAll(Metadata metadata)
	{
		foreach (TextEntry entry in GetAllFromAttributes(metadata))
			yield return entry;

		if (metadata.AlbumTrack is not null)
		{
			if (metadata.Formatting.AlbumTrackKey.HasFlag(AlbumTrackKeys.Track))
				yield return new(IniFormatting.Track, metadata.AlbumTrack.ToString()!);

			if (metadata.Formatting.AlbumTrackKey.HasFlag(AlbumTrackKeys.AlbumTrack))
				yield return new(IniFormatting.AlbumTrack, metadata.AlbumTrack.ToString()!);
		}

		if (metadata.Charter.Name is not null)
		{
			if (metadata.Formatting.CharterKey.HasFlag(CharterKeys.Charter))
				yield return new(IniFormatting.Charter, metadata.Charter.Name);

			if (metadata.Formatting.CharterKey.HasFlag(CharterKeys.Frets))
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
