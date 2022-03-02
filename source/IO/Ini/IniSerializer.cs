using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Ini
{
    internal abstract class IniSerializer : Serializer<string>
    {
        public IniSerializer(string header, WritingSession session) : base(header, session) { }

        public static string GetLine(string header, string? value) => value is null ? string.Empty : $"{header} = {value}";
    }
}
