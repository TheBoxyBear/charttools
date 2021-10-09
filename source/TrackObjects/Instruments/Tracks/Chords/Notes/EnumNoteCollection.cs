using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChartTools
{
    public class EnumNoteCollection<TNote, TLane> : NoteCollection<TNote, TLane> where TNote : Note<TLane> where TLane : struct, Enum
    {
        public EnumNoteCollection(bool openExclusivity) : base(openExclusivity) { }

        public override bool Contains(TLane lane) => Enum.IsDefined(lane) ? base.Contains(lane) : throw CommonExceptions.GetUndefinedException(lane);

        public override TNote? this[TLane lane] => Enum.IsDefined(lane) ? base[lane] : throw CommonExceptions.GetUndefinedException(lane);
    }
}
