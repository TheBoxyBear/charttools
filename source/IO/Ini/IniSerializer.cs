using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Ini
{
    internal class IniSerializer : Serializer<Metadata, string>
    {
        public IniSerializer(string header, Metadata content, WritingSession session) : base(header, content, session) { }

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
        }
    }
}
