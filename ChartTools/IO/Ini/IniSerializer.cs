using ChartTools.IO.Formatting;

namespace ChartTools.IO.Ini;

internal class IniSerializer(Metadata content) : Serializer<Metadata, string>(IniFormatting.Header, content)
{
	public override IEnumerable<string> Serialize()
	{
		if (Content is null)
			yield break;

		IEnumerable<(string key, string value)> props = IniKeySerializableAttribute.GetSerializable(Content)
			.Concat(IniKeySerializableAttribute.GetSerializable(Content.Formatting))
			.Concat(IniKeySerializableAttribute.GetSerializable(Content.Charter)
			.Concat(IniKeySerializableAttribute.GetSerializable(Content.InstrumentDifficulties)));

		foreach ((string key, string value) in props)
			yield return IniFormatting.Line(key, value.ToString());

		foreach (UnidentifiedMetadata data in Content.UnidentifiedData.Where(x => x.Origin is FileType.Ini))
			yield return IniFormatting.Line(data.Key, data.Value);

		if (Content.AlbumTrack is not null)
		{
			if (Content.Formatting.AlbumTrackKey.HasFlag(AlbumTrackKey.Track))
				yield return IniFormatting.Line(IniFormatting.Track, Content.AlbumTrack.ToString()!);

			if (Content.Formatting.AlbumTrackKey.HasFlag(AlbumTrackKey.AlbumTrack))
				yield return IniFormatting.Line(IniFormatting.AlbumTrack, Content.AlbumTrack.ToString()!);
		}
	}
}
