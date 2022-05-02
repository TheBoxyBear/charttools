using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart.Parsers
{
    internal class UnknownSectionParser : ChartParser
    {
        public override Section<string> Result => GetResult(result);
        private readonly Section<string> result;
        public UnknownSectionParser(ReadingSession session, string header) : base(session, header) => result = new(header);

        public override void ApplyToSong(Song song) => (song.UnknownChartSections ??= new()).Add(Result);
        protected override void HandleItem(string item) => result.Add(item);
    }
}
