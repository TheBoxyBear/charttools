using ChartTools.IO;
using ChartTools.IO.Formatting;
using ChartTools.IO.Ini;

namespace ChartTools.Meta.Mapping;

[MetadataMapper(FileType.Ini)]
internal partial class MetadataIniMapper : MetadataMapper
{
	public override string? Get(Metadata metadata, in ReadOnlySpan<char> key)
	{
		ValidateKey(in key);

		return TryGetFromAttribute(metadata, key, out var value)
			? value : key switch
			{
				IniFormatting.AudioOffset => metadata.AudioOffset?.TotalMilliseconds.ToString(),
				IniFormatting.VideoOffset => metadata.VideoOffset?.TotalMilliseconds.ToString(),
				IniFormatting.Modchart    => metadata.IsModchart is null ? null : metadata.IsModchart.Value ? "1" : "0",
				IniFormatting.Track when metadata.Formatting.AlbumTrackKeys.HasFlag(AlbumTrackKeys.Track)
					=> metadata.AlbumTrack?.ToString(),
				IniFormatting.AlbumTrack when metadata.Formatting.AlbumTrackKeys.HasFlag(AlbumTrackKeys.AlbumTrack)
					=> metadata.AlbumTrack?.ToString(),
				IniFormatting.Charter when metadata.Formatting.CharterKeys.HasFlag(CharterKeys.Charter)
					=> metadata.Charter.Name,
				IniFormatting.Frets when metadata.Formatting.CharterKeys.HasFlag(CharterKeys.Frets)
					=> metadata.Charter.Name,
				_ => FindUndentified(metadata, key)
			};
	}

	public override void Set(Metadata metadata, in ReadOnlySpan<char> key, in ReadOnlySpan<char> value)
	{
		ValidateKey(in key);

		if (!TrySetFromAttribute(metadata, in key, in value))
			switch (key)
			{
				case IniFormatting.AudioOffset:
					metadata.AudioOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<float>(value, nameof(Metadata.AudioOffset)));
					break;
				case IniFormatting.VideoOffset:
					metadata.VideoOffset = TimeSpan.FromMilliseconds(ValueParser.Parse<float>(value, nameof(Metadata.VideoOffset)));
					break;
				case IniFormatting.Modchart:
					metadata.IsModchart = ValueParser.Parse<bool>(value, nameof(Metadata.IsModchart));
					break;
				case IniFormatting.AlbumTrack:
					metadata.AlbumTrack = ValueParser.Parse<byte>(in value, nameof(Metadata.AlbumTrack));
					metadata.Formatting.AlbumTrackKeys |= AlbumTrackKeys.AlbumTrack;
					break;
				case IniFormatting.Track:
					metadata.AlbumTrack = ValueParser.Parse<byte>(in value, nameof(Metadata.AlbumTrack));
					metadata.Formatting.AlbumTrackKeys |= AlbumTrackKeys.Track;
					break;
				case IniFormatting.Charter:
					metadata.Charter.Name = value.ToString();
					metadata.Formatting.CharterKeys |= CharterKeys.Charter;
					break;
				case IniFormatting.Frets:
					metadata.Charter.Name = value.ToString();
					metadata.Formatting.CharterKeys |= CharterKeys.Frets;
					break;
				default:
					AddUnidentified(metadata, in key, in value);
					break;
			}
	}

	public override void Remove(Metadata metadata, in ReadOnlySpan<char> key)
	{
		ValidateKey(in key);

		if (!TryRemoveFromAttribute(metadata, in key));
			switch (key)
			{
				case IniFormatting.Track:
					if ((metadata.Formatting.AlbumTrackKeys &= ~AlbumTrackKeys.Track) == AlbumTrackKeys.None)
						metadata.AlbumTrack = null;
					break;
				case IniFormatting.AlbumTrack:
					if ((metadata.Formatting.AlbumTrackKeys &= ~AlbumTrackKeys.AlbumTrack) == AlbumTrackKeys.None)
						metadata.AlbumTrack = null;
					break;
				case IniFormatting.Charter:
					if ((metadata.Formatting.CharterKeys &= ~CharterKeys.Charter) == CharterKeys.None)
						metadata.Charter.Name = null;
					break;
				case IniFormatting.Frets:
					if ((metadata.Formatting.CharterKeys &= ~CharterKeys.Frets) == CharterKeys.None)
						metadata.Charter.Name = null;
				break;
				default:
					RemoveUnidentified(metadata, in key);
					break;
			}
	}


	public override bool Contains(Metadata metadata, in ReadOnlySpan<char> key)
	{
		ValidateKey(in key);

		return TryContainsFromAttribute(metadata, in key) ?? key switch
		{
			IniFormatting.AudioOffset => metadata.AudioOffset is not null,
			IniFormatting.VideoOffset => metadata.VideoOffset is not null,
			IniFormatting.Modchart    => metadata.IsModchart is not null,
			IniFormatting.Track when metadata.Formatting.AlbumTrackKeys.HasFlag(AlbumTrackKeys.Track)
				=> metadata.AlbumTrack is not null,
			IniFormatting.AlbumTrack when metadata.Formatting.AlbumTrackKeys.HasFlag(AlbumTrackKeys.AlbumTrack)
				=> metadata.AlbumTrack is not null,
			IniFormatting.Charter when metadata.Formatting.CharterKeys.HasFlag(CharterKeys.Charter)
				=> metadata.Charter.Name is not null,
			IniFormatting.Frets when metadata.Formatting.CharterKeys.HasFlag(CharterKeys.Frets)
				=> metadata.Charter.Name is not null,
			_ => ContainsUnidentified(metadata, in key)
		};
	}

	public override IEnumerable<TextEntry> GetAll(Metadata metadata)
	{
		foreach (TextEntry entry in GetAllFromAttributes(metadata))
			yield return entry;

		foreach (string key in new string[] { IniFormatting.AudioOffset, IniFormatting.VideoOffset, IniFormatting.Modchart })
		{
			string? value = Get(metadata, key);

			if (value is not null)
				yield return new(key, value);
		}

		string? audioOffset = Get(metadata, IniFormatting.AudioOffset);

		if (audioOffset is not null)
			yield return new(IniFormatting.AudioOffset, audioOffset);

		if (metadata.AlbumTrack is not null)
		{
			if (metadata.Formatting.EffectiveAlbumTrackKeys.HasFlag(AlbumTrackKeys.Track))
				yield return new(IniFormatting.Track, metadata.AlbumTrack.ToString()!);

			if (metadata.Formatting.EffectiveAlbumTrackKeys.HasFlag(AlbumTrackKeys.AlbumTrack))
				yield return new(IniFormatting.AlbumTrack, metadata.AlbumTrack.ToString()!);
		}

		if (metadata.Charter.Name is not null)
		{
			if (metadata.Formatting.EffectiveCharterKeys.HasFlag(CharterKeys.Charter))
				yield return new(IniFormatting.Charter, metadata.Charter.Name);

			if (metadata.Formatting.EffectiveCharterKeys.HasFlag(CharterKeys.Frets))
				yield return new(IniFormatting.Frets, metadata.Charter.Name.ToString());
		}

		foreach (TextEntry entry in GetAllUnidentified(metadata))
			yield return entry;
	}
}
