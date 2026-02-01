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
	public ChartReadingSession Session { get; } = session;

	public override IEnumerable<ChartParser> Parsers
		=> base.Parsers.Cast<ChartParser>();

	public override bool DefinedSectionEnd => true;

	public Metadata? ExistingMetadata { get; set; }

	protected override ChartParser? GetParser(string header)
	{
		switch (header)
		{
			case ChartFormatting.MetadataHeader:
				return Session.Components.Metadata ? new MetadataParser(ExistingMetadata) : null;
			case ChartFormatting.GlobalEventHeader:
				// Vocals are read from global events in chart files. Gets converted to vocals when assembling the song object
				return Session.Components.GlobalEvents || Session.Components.Vocals ? new GlobalEventParser(Session) : null;
			case ChartFormatting.SyncTrackHeader:
				return Session.Components.SyncTrack ? new SyncTrackParser(Session) : null;
			default:
				if (ChartFormatting.DrumsTrackHeaders.TryGetValue(header, out Difficulty diff))
					return Session.Components.Instruments.Drums.HasFlag(diff.ToSet())
						? new DrumsTrackParser(diff, Session, header) : null;
				else if (ChartFormatting.GHLTrackHeaders.TryGetValue(header, out (Difficulty, GHLInstrumentIdentity) ghlTuple))
					return Session.Components.Instruments.Map(ghlTuple.Item2).HasFlag(ghlTuple.Item1.ToSet())
						? new GHLTrackParser(ghlTuple.Item1, ghlTuple.Item2, Session, header) : null;
				else if (ChartFormatting.StandardTrackHeaders.TryGetValue(header, out (Difficulty, StandardInstrumentIdentity) standardTuple))
					return Session.Components.Instruments.Map(standardTuple.Item2).HasFlag(standardTuple.Item1.ToSet())
						? new StandardTrackParser(standardTuple.Item1, standardTuple.Item2, Session, header) : null;
				else
				{
					return Session.Configuration.UnknownSectionPolicy == UnknownSectionPolicy.ThrowException
						? throw new Exception($"Unknown section with header \"{header}\". Consider using {UnknownSectionPolicy.Store} to avoid this error.")
						: new UnknownSectionParser(Session, header);
				}
		}
	}

	protected override bool IsSectionStart(string line)
		=> line == "{";

	protected override bool IsSectionEnd(string line)
		=> ChartFormatting.IsSectionEnd(line);
}
