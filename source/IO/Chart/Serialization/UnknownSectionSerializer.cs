using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Sections;
using ChartTools.IO.Serializaiton;

using System.Collections.Generic;

namespace ChartTools.IO.Chart.Serialization
{
    internal class UnknownSectionSerializer : Serializer<Section<string>, string>
    {
        public UnknownSectionSerializer(string header, Section<string> content, WritingSession session) : base(header, content, session) { }

        public override IEnumerable<string> Serialize() => Content;
    }
}
