using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Parsing;
using ChartTools.IO.Components;
using ChartTools.IO.Configuration;
using ChartTools.IO.Parsing;
using ChartTools.IO.Sources;
using ChartTools.Meta;

namespace ChartTools.IO.Chart;

/// <summary>
/// Reader of text file that sends read lines to subscribers of its events.
/// </summary>
internal class ChartFileReader(ReadingDataSource source, ChartReadingSession session)
	: TextFileReader(source)
{
	public ChartReadingSession Session { get; } = session;

#if NET5_0_OR_GREATER
	public override IEnumerable<ChartParser> Parsers
#else
	public new IEnumerable<ChartParser> Parsers
#endif
		=> base.Parsers.Cast<ChartParser>();

	public override bool DefinedSectionEnd => true;

	public Metadata? ExistingMetadata { get; set; }

	protected override TextParser? GetParser(in ReadOnlyMemory<char> header)
	{
		string headerString = header.ToString();

		switch (header.Span)
		{
			case ChartFormatting.MetadataHeader:
				return Session.Components.Metadata ? new MetadataParser(ExistingMetadata) : null;
			case ChartFormatting.GlobalEventHeader:
				// Vocals are read from global events in chart files. Gets converted to vocals when assembling the song object
				return Session.Components.GlobalEvents || Session.Components.Vocals ? new GlobalEventParser(Session) : null;
			case ChartFormatting.SyncTrackHeader:
				return Session.Components.SyncTrack ? new SyncTrackParser(Session) : null;
			default:
				if (ChartFormatting.DrumsTrackHeaders.TryGetValue(headerString, out Difficulty diff))
					return Session.Components.Instruments.Drums.HasFlag(diff.ToSet())
						? new DrumsTrackParser(diff, Session, in header) : null;
				else if (ChartFormatting.GHLTrackHeaders.TryGetValue(headerString, out (Difficulty, GHLInstrumentIdentity) ghlTuple))
					return Session.Components.Instruments.Map(ghlTuple.Item2).HasFlag(ghlTuple.Item1.ToSet())
						? new GHLTrackParser(ghlTuple.Item1, ghlTuple.Item2, Session, in header) : null;
				else if (ChartFormatting.StandardTrackHeaders.TryGetValue(headerString, out (Difficulty, StandardInstrumentIdentity) standardTuple))
					return Session.Components.Instruments.Map(standardTuple.Item2).HasFlag(standardTuple.Item1.ToSet())
						? new StandardTrackParser(standardTuple.Item1, standardTuple.Item2, Session, in header) : null;
				else
				{
					return Session.Configuration.UnknownSectionPolicy is UnknownSectionPolicy.ThrowException
						? throw new Exception($"Unknown section with header \"{header}\". Consider using {UnknownSectionPolicy.Store} to avoid this error.")
						: new UnknownSectionParser(Session, in header);
				}
		}
	}

	protected override bool IsSectionStart(in ReadOnlySpan<char> line)
		=> line is "{";

	protected override bool IsSectionEnd(in ReadOnlySpan<char> line)
		=> ChartFormatting.IsSectionEnd(line);
}
