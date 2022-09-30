﻿using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Configuration.Sessions;

using System.Collections.Generic;

namespace ChartTools
{
    public abstract class LaneChord : TrackObjectBase, IChord
    {
        public IEnumerable<LaneNote> Notes => GetNotes();
        IEnumerable<INote> IChord.Notes => GetNotes();

        internal abstract bool ChartSupportedModifiers { get; }

        public LaneChord() : base() { }
        public LaneChord(uint position) : base(position) { }

        public abstract LaneNote CreateNote(byte index);
        INote IChord.CreateNote(byte index) => CreateNote(index);

        protected abstract IEnumerable<LaneNote> GetNotes();

        internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();
        internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, WritingSession session);
    }
}
