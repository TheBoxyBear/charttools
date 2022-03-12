using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ChartTools.IO.Ini
{
    internal class IniSerializer : Serializer<Metadata, string>
    {
        public IniSerializer(string header, Metadata content, WritingSession session) : base(header, content, session) { }

        public override IEnumerable<string> Serialize()
        {
            var props = GetSerializable(Content).Concat(GetSerializable(Content.Formatting)).Concat(GetSerializable(Content.Charter)).Concat(GetSerializable(Content.InstrumentDifficulties));

            // Generates groups of non-null property values and their serialization keys
            static IEnumerable<(string key, string value)> GetSerializable(object source) => from prop in source.GetType().GetProperties()
                                                                                                       let att = prop.GetCustomAttribute<IniSimpleSerializeAttribute>()
                                                                                                       where att is not null
                                                                                                       let value = prop.GetValue(source)?.ToString()
                                                                                                       where value is not null
                                                                                                       select (att.Key, value);

            foreach ((var key, var value) in props)
                yield return IniFormatting.Line(key, value.ToString());

            foreach (var data in Content.UnidentifiedData)
                yield return IniFormatting.Line(data.Key, data.Value);
        }
    }
}
