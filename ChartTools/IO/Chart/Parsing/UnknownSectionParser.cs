using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Sections;

namespace ChartTools.IO.Chart.Parsing;

internal class UnknownSectionParser(ChartReadingSession session, string header) : ChartParser(session, header)
{
    public override Section<string> Result => GetResult(result);
    private readonly Section<string> result = new(header);

    public override void ApplyToSong(Song song) => (song.UnknownChartSections ??= []).Add(Result);
    protected override void HandleItem(string item) => result.Add(item);
}
