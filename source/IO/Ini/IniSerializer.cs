using ChartTools.Formatting;
using ChartTools.IO.Serializaiton;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Ini
{
    internal class IniSerializer : Serializer<Metadata, string>
    {
        public IniSerializer(Metadata content) : base(IniFormatting.Header, content, null!) { }

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

            foreach (var data in Content.UnidentifiedData)
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
}
