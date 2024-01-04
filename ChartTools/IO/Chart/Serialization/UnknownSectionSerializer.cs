using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart.Serialization;

internal class UnknownSectionSerializer(string header, Section<string> content, ChartWritingSession session) : Serializer<Section<string>, string>(header, content)
{
    public ChartWritingSession Session { get; } = session;

    public override IEnumerable<string> Serialize() => Content;
}
