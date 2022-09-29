using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Formatting;

using System.Collections.Generic;

namespace ChartTools
{
    public abstract class LaneChord : TrackObjectBase, IChord
    {
        public IEnumerable<INote> Notes => GetNotes();
        internal abstract bool ChartSupportedModifiers { get; }

        public LaneChord() : base() { }
        public LaneChord(uint position) : base(position) { }

        INote IChord.CreateNote(byte index, uint sustain) => CreateNote(index, sustain);
        protected abstract INote CreateNote(byte index, uint sustain = 0);
        protected abstract IEnumerable<INote> GetNotes();

        internal abstract IEnumerable<TrackObjectEntry> GetChartData(LaneChord? previous, bool modifiers, FormattingRules formatting);
    }
}
