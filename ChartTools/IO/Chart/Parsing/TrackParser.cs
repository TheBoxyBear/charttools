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

    private TChord? chord;
    private bool newChord = true;
    private readonly List<TChord> orderedChords = [];

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
                var newIndex = 0;

                // Find the parent chord or create it
                if (chord is null)
                {
                    chord = new() { Position = entry.Position };
                    newIndex = orderedChords.Count;
                }
                else if (entry.Position == chord.Position)
                    newChord = false;
                else
                {
                    newIndex = orderedChords.BinarySearchIndex(entry.Position, c => c.Position, out bool exactMatch);

                    if (newChord = !exactMatch)
                        chord = new() { Position = entry.Position };
                }

                HandleNoteEntry(chord!, new(entry.Data));

                if (newChord)
                {
                    result.Chords.Add(chord!);
                    orderedChords.Insert(newIndex, chord!);
                }

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
        if (session.HandleDuplicate(chord!.Position, "note", () => chord!.Notes.Any(n => n.Index == note.Index)))
            add();
    }
    protected void HandleAddModifier(Enum existingModifier, Enum modifier, Action add)
    {
        if (session.HandleDuplicate(chord!.Position, "chord modifier", () => existingModifier.HasFlag(modifier)))
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
