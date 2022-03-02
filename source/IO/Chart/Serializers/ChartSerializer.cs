using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class ChartSerializer : Serializer<string>
    {
        public ChartSerializer(string header, WritingSession session) : base(header, session) { }

        public static string GetLine(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";
    }
}
