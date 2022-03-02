using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO
{
    internal abstract class TextSerializer : Serializer<string>
    {
        protected TextSerializer(string header, WritingSession session) : base(header, session) { }

        internal static string GetLine(string header, string? value) => value is null ? string.Empty : $"  {header} = {value}";
    }
}
