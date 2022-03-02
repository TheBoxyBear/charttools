using ChartTools.IO.Configuration.Sessions;

namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class ChartSerializer : TextSerializer
    {
        public ChartSerializer(string header, WritingSession session) : base(header, session) { }
    }
}
