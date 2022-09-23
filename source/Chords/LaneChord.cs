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

        protected LaneChord() : base() => Notes = new(OpenExclusivity);
        /// <inheritdoc cref="Chord{TNote, Tlane, TModifier}(uint)"/>
        protected LaneChord(uint position) : base(position) => Notes = new(OpenExclusivity);

        public override Note<TLane> CreateNote(byte index, uint length = 0)
        {
            var note = new TNote()
            {
                Lane = Unsafe.As<byte, TLane>(ref index),
                Length = length
            };

            Notes.Add(note);
            return note;
        }
        internal abstract IEnumerable<TrackObjectEntry> GetChartNoteData();
        internal abstract IEnumerable<TrackObjectEntry> GetChartModifierData(LaneChord? previous, WritingSession session);
    }
}
