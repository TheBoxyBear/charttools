using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.Tools;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class TrackParser<TChord>(Difficulty difficulty, ChartReadingSession session, in ReadOnlyMemory<char> header)
	: ChartParser(session, in header), IInstrumentAppliable<TChord>
	where TChord : Chord, new()
{
	public Difficulty Difficulty { get; } = difficulty;

#if NET5_0_OR_GREATER
	public override Track<TChord> Result
		=> GetResultIfReady(m_result);
#else
	public new Track<TChord> Result
		=> GetResultIfReady(m_result);

	protected override object? GetResult()
		=> Result;
#endif

	private readonly Track<TChord> m_result = new() { Difficulty = difficulty };

	private TChord? m_currentChord;

	protected override void HandleItem(in ReadOnlyMemory<char> line)
	{
		TrackObjectEntry entry = new(line);

		// Can be optimized by switching on the single char
		switch (entry.Type.Span)
		{
			// Local event
			case "E":
				m_result.LocalEvents.Add(new(entry.Position, entry.Data.ToString()));
				break;
			// Note or chord modifier
			case "N":
				// Find the parent chord or create it
				if (m_currentChord is null) // First chord
				{
					m_currentChord = new() { Position = entry.Position };
					m_result.Chords.Add(m_currentChord);
				}
				// Start of a new chord or the note belonging to an existing chord is misplaced
				else if (entry.Position != m_currentChord.Position)
				{
					// Notes are typically in order of position, not requiring a search for an existing chord
					if (entry.Position > m_result.Chords[^1].Position) // New chord
					{
						m_currentChord = new() { Position = entry.Position };
						m_result.Chords.Add(m_currentChord);
					}
					else // Misplaced note - Requires search for the parent chord
					{
						int index = m_result.Chords.BinarySearchIndex(entry.Position, static c => c.Position, out bool exactMatch);

						if (exactMatch)
							m_currentChord = m_result.Chords[index];
						else
						{
							m_currentChord = new() { Position = entry.Position };
							m_result.Chords.Insert(index, m_currentChord);
						}
					}
				}

				HandleNoteEntry(m_currentChord, new(entry.Data.Span));

				break;
			// Special phrase
			case "S":
				ReadOnlySpan<char> a, b;
				ChartFormatting.SplitData(entry.Data.Span, out a, out b);

				byte typeCode = ValueParser.Parse<byte>(in a, "type code");
				uint length   = ValueParser.Parse<uint>(in b, "length");

				m_result.SpecialPhrases.Add(new(entry.Position, typeCode, length));
				break;
		}
	}

	protected abstract void HandleNoteEntry(TChord chord, in NoteData data);

    protected override void FinaliseParse()
    {
        ApplyOverlappingSpecialPhrasePolicy(result.SpecialPhrases, session!.Configuration.OverlappingSpecialPhrasePolicy);
        base.FinaliseParse();
    }
	
	protected bool CanAddNote(byte index)
		=> Session.HandleDuplicate(m_currentChord!.Position, "note",
			() => m_currentChord.Notes.AsEnumerable().Any(n => n.Index == index));

	protected bool CanAddModifier(Enum existingModifier, Enum modifier)
		=> Session.HandleDuplicate(m_currentChord!.Position, "chord modifier",
			() => existingModifier.HasFlag(modifier));

	protected override void FinalizeParse()
	{
		if (Session.Configuration.SoloNoStarPowerPolicy is SoloNoStarPowerPolicy.Convert
			&& !m_result.SpecialPhrases.Any(sp => sp.Type is TrackSpecialPhraseType.StarPowerGain))
			m_result.SpecialPhrases.AddRange(m_result.SoloToStarPower(true));

		ApplyOverlappingSpecialPhrasePolicy(m_result.SpecialPhrases, Session.Configuration.OverlappingStarPowerPolicy);
		base.FinalizeParse();
	}

	public void ApplyToInstrument(Instrument<TChord> instrument)
		=> instrument.SetTrack(Result);

	private static void ApplyOverlappingSpecialPhrasePolicy(IEnumerable<TrackSpecialPhrase> specialPhrases, OverlappingSpecialPhrasePolicy policy)
	{
		switch (policy)
		{
			case OverlappingSpecialPhrasePolicy.Cut:
				specialPhrases.CutLengths();
				break;
			case OverlappingSpecialPhrasePolicy.ThrowException:
				foreach ((TrackSpecialPhrase previous, TrackSpecialPhrase current) in specialPhrases.RelativeLoopSkipFirst())
					if (Optimizer.LengthNeedsCut(previous, current))
						throw new Exception($"Overlapping star power phrases at position {current.Position}. Consider using {nameof(OverlappingSpecialPhrasePolicy.Cut)} to avoid this error.");
				break;
		}
	}
}
