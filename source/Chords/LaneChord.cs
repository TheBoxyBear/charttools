using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Formatting;

namespace ChartTools;

public abstract class LaneChord : TrackObjectBase, IChord
{
    public IReadOnlyCollection<LaneNote> Notes => GetNotes();
    IReadOnlyCollection<INote> IChord.Notes => GetNotes();

    /// <summary>
    /// Defines if open notes can be mixed with other notes for this chord type. <see langword="true"/> indicated open notes cannot be mixed.
    /// </summary>
    public abstract bool OpenExclusivity { get; }

    internal abstract bool ChartSupportedModifiers { get; }

        public LaneChord() : base() { }
        protected LaneChord(uint position) : base(position) { }

    public abstract LaneNote CreateNote(byte index, uint sustain = 0);
    INote IChord.CreateNote(byte index, uint sustain) => CreateNote(index, sustain);

    protected abstract IReadOnlyCollection<LaneNote> GetNotes();

        internal abstract IEnumerable<TrackObjectEntry> GetChartData(LaneChord? previous, bool modifiers, FormattingRules formatting);
    }
}
