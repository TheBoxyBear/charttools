using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.Tools;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class TrackParser<TChord>(Difficulty difficulty, ChartReadingSession session, string header)
	: ChartParser(session, header), IInstrumentAppliable<TChord>
	where TChord : Chord, new()
{
	public Difficulty Difficulty { get; } = difficulty;

	public override Track<TChord> Result
		=> GetResult(result);

	private readonly Track<TChord> result = new() { Difficulty = difficulty };

	private TChord? currentChord;

	protected override void HandleItem(string line)
	{
		TrackObjectEntry entry = new(line);

		// Can be optimized by switching on the single char
		switch (entry.Type)
		{
			// Local event
			case "E":
				result.LocalEvents.Add(new(entry.Position, entry.Data));
				break;
			// Note or chord modifier
			case "N":
				// Find the parent chord or create it
				if (currentChord is null) // First chord
				{
					currentChord = new() { Position = entry.Position };
					result.Chords.Add(currentChord);
				}
				// Start of a new chord or the note belonging to an existing chord is misplaced
				else if (entry.Position != currentChord.Position)
				{
					// Notes are typically in order of position, not requiring a search for an existing chord
					if (entry.Position > result.Chords[^1].Position) // New chord
					{
						currentChord = new() { Position = entry.Position };
						result.Chords.Add(currentChord);
					}
					else // Misplaced note - Requires search for the parent chord
					{
						int index = result.Chords.BinarySearchIndex(entry.Position, c => c.Position, out bool exactMatch);

						if (exactMatch)
							currentChord = result.Chords[index];
						else
						{
							currentChord = new() { Position = entry.Position };
							result.Chords.Insert(index, currentChord);
						}
					}
				}

				HandleNoteEntry(currentChord, new(entry.Data));

				break;
			// Special phrase
			case "S":
				string[] split = ChartFormatting.SplitData(entry.Data);

				byte typeCode = ValueParser.Parse<byte>(split[0], "type code");
				uint length   = ValueParser.Parse<uint>(split[1], "length");

				result.SpecialPhrases.Add(new(entry.Position, typeCode, length));
				break;
		}
	}

	protected abstract void HandleNoteEntry(TChord chord, in NoteData data);

	protected bool CanAddNote(byte index)
		=> Session.HandleDuplicate(currentChord!.Position, "note",
			() => currentChord.Notes.AsEnumerable().Any(n => n.Index == index));

	protected bool CanAddModifier(Enum existingModifier, Enum modifier)
		=> Session.HandleDuplicate(currentChord!.Position, "chord modifier",
			() => existingModifier.HasFlag(modifier));

	protected override void FinalizeParse()
	{
		if (Session.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert
			&& !result.SpecialPhrases.Any(sp => sp.Type is TrackSpecialPhraseType.StarPowerGain))
			result.SpecialPhrases.AddRange(result.SoloToStarPower(true));

		ApplyOverlappingSpecialPhrasePolicy(result.SpecialPhrases, Session.Configuration.OverlappingStarPowerPolicy);
		base.FinalizeParse();
	}

	public void ApplyToInstrument(Instrument<TChord> instrument) => instrument.SetTrack(Result);

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
