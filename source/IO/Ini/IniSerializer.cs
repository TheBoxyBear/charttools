using ChartTools.Formatting;
using ChartTools.IO.Configuration.Sessions;

using System;
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
            var metadataProps = GetSerializable(typeof(Metadata), Content);
            var formattingProps = GetSerializable(typeof(FormattingRules), Content.Formatting);
            var charterProps = GetSerializable(typeof(Charter), Content.Charter);

            static IEnumerable<(string key, string value)> GetSerializable(Type type, object source) => from prop in type.GetProperties()
                                                                                                        let att = prop.GetCustomAttribute<IniSimpleSerializeAttribute>()
                                                                                                        where att is not null
                                                                                                        let value = prop.GetValue(source)?.ToString()
                                                                                                        where value is not null
                                                                                                        select (att.Key, value);

            foreach ((var key, var value) in metadataProps.Concat(formattingProps).Concat(charterProps))
                yield return IniFormatting.Line(key, value.ToString());

            foreach (var data in Content.UnidentifiedData)
                yield return IniFormatting.Line(data.Key, data.Value);
        }
    }
}
