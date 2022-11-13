﻿using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart.Serializing;

internal class UnknownSectionSerializer : Serializer<Section<string>, string>
{
    public UnknownSectionSerializer(string header, Section<string> content, WritingSession session) : base(header, content, session) { }

    public override IEnumerable<string> Serialize() => Content;
}
