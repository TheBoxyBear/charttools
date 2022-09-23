using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools
{
    public abstract class LaneChord : TrackObjectBase, IChord
    {
        public IEnumerable<INote> Notes => GetNotes();
        internal abstract bool ChartSupportedModifiers { get; }

        public LaneChord() : base() { }
        public LaneChord(uint position) : base(position) { }

        protected abstract IEnumerable<INote> GetNotes();

        internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();
        internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, WritingSession session);
    }
}
