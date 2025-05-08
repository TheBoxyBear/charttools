using ChartTools.IO.Formatting;

namespace ChartTools.IO.Ini;

internal class IniSerializer(Metadata content) : Serializer<Metadata, string>(IniFormatting.Header, content)
{
	public override IEnumerable<string> Serialize()
	{
		if (Content is null)
			yield break;

		var props = IniKeySerializableAttribute.GetSerializable(Content)
			.Concat(IniKeySerializableAttribute.GetSerializable(Content.Formatting))
			.Concat(IniKeySerializableAttribute.GetSerializable(Content.Charter)
			.Concat(IniKeySerializableAttribute.GetSerializable(Content.InstrumentDifficulties)));

		foreach ((var key, var value) in props)
			yield return IniFormatting.Line(key, value.ToString());

		foreach (var data in Content.UnidentifiedData.Where(x => x.Origin is FileType.Ini))
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
