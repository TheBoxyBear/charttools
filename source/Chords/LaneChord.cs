using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Formatting;

using System.Collections.Generic;

namespace ChartTools
{
    public abstract class LaneChord : TrackObjectBase, IChord
    {
        public IEnumerable<INote> Notes => GetNotes();
        internal abstract bool ChartSupportedModifiers { get; }

        protected abstract IEnumerable<INote> GetNotes();

        public LaneChord() : base() { }
        protected LaneChord(uint position) : base(position) { }

        protected abstract INote CreateNote(byte index, uint sustain);
        INote IChord.CreateNote(byte index, uint sustain) => CreateNote(index, sustain);

        internal abstract IEnumerable<TrackObjectEntry> GetChartData(LaneChord? previous, bool modifiers, FormattingRules formatting);
    }
}
