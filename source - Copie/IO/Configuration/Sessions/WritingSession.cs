using ChartTools.IO.Formatting;
using ChartTools.IO.Chart.Entries;

namespace ChartTools.IO.Configuration.Sessions;

internal class WritingSession : Session
{
    public delegate IEnumerable<TrackObjectEntry> ChordEntriesGetter(LaneChord? previous, LaneChord current);

    public override WritingConfiguration Configuration { get; }
    public ChordEntriesGetter GetChordEntries { get; private set; }

    public WritingSession(WritingConfiguration config, FormattingRules? formatting) : base(formatting)
    {
        Configuration = config;
        GetChordEntries = (previous, chord) => (GetChordEntries = Configuration.UnsupportedModifierPolicy switch
        {
            UnsupportedModifierPolicy.IgnoreChord => (_, _) => Enumerable.Empty<TrackObjectEntry>(),
            UnsupportedModifierPolicy.ThrowException => (_, chord) => throw new Exception($"Chord at position {chord.Position} as an unsupported modifier for the chart format."),
            UnsupportedModifierPolicy.IgnoreModifier => (_, chord) => chord.GetChartNoteData(),
            UnsupportedModifierPolicy.Convert => (previous, chord) => chord.GetChartModifierData(previous, this),
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnsupportedModifierPolicy)
        })(previous, chord);
    }
}
