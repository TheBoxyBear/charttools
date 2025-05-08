using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Configuration.Sessions;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration;
using ChartTools.Tools;

namespace ChartTools.IO.Chart.Parsing;

internal abstract class TrackParser<TChord>(Difficulty difficulty, ChartReadingSession session, string header)
	: ChartParser(session, header), IInstrumentAppliable<TChord> where TChord : IChord, new()
{
	public Difficulty Difficulty { get; } = difficulty;

	public override Track<TChord> Result => GetResult(result);
	private readonly Track<TChord> result = new() { Difficulty = difficulty };

	private TChord? currentChord;

	protected override void HandleItem(string line)
	{
		TrackObjectEntry entry = new(line);

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
					result.Chords.Add(currentChord!);
				}
				// Start of a new chord or the note belonging to an existing chord is misplaced
				else if (entry.Position != currentChord.Position)
				{
					// Notes are typically in order of position, not requiring a search for an existing chord
					if (entry.Position > result.Chords[^1].Position) // New chord
					{
						currentChord = new() { Position = entry.Position };
						result.Chords.Add(currentChord!);
					}
					else // Misplaced note - Requires search for the parent chord
					{
						var index = result.Chords.BinarySearchIndex(entry.Position, c => c.Position, out bool exactMatch);

						if (exactMatch)
							currentChord = result.Chords[index];
						else
						{
							currentChord = new() { Position = entry.Position };
							result.Chords.Insert(index, currentChord!);
						}
					}
				}

				HandleNoteEntry(currentChord!, new(entry.Data));

				break;
			// Star power
			case "S":
				var split = ChartFormatting.SplitData(entry.Data);

				var typeCode = ValueParser.ParseByte(split[0], "type code");
				var length = ValueParser.ParseUint(split[1], "length");

				result.SpecialPhrases.Add(new(entry.Position, typeCode, length));
				break;
		}

		if (session!.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert)
			result.SpecialPhrases.AddRange(result.SoloToStarPower(true));
	}

	protected abstract void HandleNoteEntry(TChord chord, NoteData data);
	protected void HandleAddNote(INote note, Action add)
	{
		if (session.HandleDuplicate(currentChord!.Position, "note", () => currentChord!.Notes.Any(n => n.Index == note.Index)))
			add();
	}
	protected void HandleAddModifier(Enum existingModifier, Enum modifier, Action add)
	{
		if (session.HandleDuplicate(currentChord!.Position, "chord modifier", () => existingModifier.HasFlag(modifier)))
			add();
	}

	protected override void FinaliseParse()
	{
		ApplyOverlappingSpecialPhrasePolicy(result.SpecialPhrases, session!.Configuration.OverlappingStarPowerPolicy);
		base.FinaliseParse();
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
				foreach ((var previous, var current) in specialPhrases.RelativeLoopSkipFirst())
					if (Optimizer.LengthNeedsCut(previous, current))
						throw new Exception($"Overlapping star power phrases at position {current!.Position}. Consider using {nameof(OverlappingSpecialPhrasePolicy.Cut)} to avoid this error.");
				break;
		}
	}
}
