using ChartTools.Collections.Unique;

using System;
using System.Collections.Generic;

namespace ChartTools
{
    public class LaneNoteCollection<TNote, TLane> : NoteCollection<TNote, TLane> where TNote : Note<TLane>, new() where TLane : struct, Enum
    {
        protected override ICollection<TNote> Notes { get; }

        /// <summary>
        /// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
        /// </summary>
        public bool OpenExclusivity { get; }

        public LaneNoteCollection(bool openExclusivity)
        {
            OpenExclusivity = openExclusivity;
            Notes = new UniqueList<TNote>((a, b) => a.NoteIndex.Equals(b.NoteIndex));
        }

        public override void Add(TLane lane)
        {
            if (Enum.IsDefined(lane))
            {
                base.Add(lane);

                if (OpenExclusivity)
                {
                    if (lane.Equals(0))
                        Notes.Clear();
                    else
                        base.Remove(lane);

                    base.Add(lane);
                }
            }
            else
                throw CommonExceptions.GetUndefinedException(lane);
        }
        public override bool Contains(TLane lane) => Enum.IsDefined(lane) ? base.Contains(lane) : throw CommonExceptions.GetUndefinedException(lane);
        public override bool Remove(TLane lane) => Enum.IsDefined(lane)? base.Remove(lane) : throw CommonExceptions.GetUndefinedException(lane);

        public override TNote? this[TLane lane] => Enum.IsDefined(lane) ? base[lane] : throw CommonExceptions.GetUndefinedException(lane);
    }
}
