using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Parsing;
using ChartTools.IO.Components;
using ChartTools.IO.Configuration;
using ChartTools.IO.Sources;

namespace ChartTools.IO.Chart;

/// <summary>
/// Reader of text file that sends read lines to subscribers of its events.
/// </summary>
internal class ChartFileReader(ReadingDataSource source, ChartReadingSession session) : TextFileReader(source)
{
    protected readonly ChartReadingSession session = session;

    public override IEnumerable<ChartParser> Parsers => base.Parsers.Cast<ChartParser>();
    public override bool DefinedSectionEnd => true;

    protected override ChartParser? GetParser(string header)
    {
        switch (header)
        {
            case ChartFormatting.MetadataHeader:
                return session.Components.Metadata ? new MetadataParser() : null;
            case ChartFormatting.GlobalEventHeader:
                return session.Components.GlobalEvents ? new GlobalEventParser(session) : null;
            case ChartFormatting.SyncTrackHeader:
                return session.Components.SyncTrack ? new SyncTrackParser(session) : null;
            default:
                if (ChartFormatting.DrumsTrackHeaders.TryGetValue(header, out Difficulty diff))
                    return session.Components.Instruments.Drums.HasFlag(diff.ToSet())
                        ? new DrumsTrackParser(diff, session, header) : null;
                else if (ChartFormatting.GHLTrackHeaders.TryGetValue(header, out (Difficulty, GHLInstrumentIdentity) ghlTuple))
                    return session.Components.Instruments.Map(ghlTuple.Item2).HasFlag(ghlTuple.Item1.ToSet())
                        ? new GHLTrackParser(ghlTuple.Item1, ghlTuple.Item2, session, header) : null;
                else if (ChartFormatting.StandardTrackHeaders.TryGetValue(header, out (Difficulty, StandardInstrumentIdentity) standardTuple))
                    return session.Components.Instruments.Map(standardTuple.Item2).HasFlag(standardTuple.Item1.ToSet())
                        ? new StandardTrackParser(standardTuple.Item1, standardTuple.Item2, session, header) : null;
                else
                {
                    return session.Configuration.UnknownSectionPolicy == UnknownSectionPolicy.ThrowException
                        ? throw new Exception($"Unknown section with header \"{header}\". Consider using {UnknownSectionPolicy.Store} to avoid this error.")
                        : new UnknownSectionParser(session, header);
                }
        }
    }

    protected override bool IsSectionStart(string line) => line == "{";
    protected override bool IsSectionEnd(string line) => ChartFormatting.IsSectionEnd(line);
}
