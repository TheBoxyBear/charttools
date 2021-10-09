using System;

namespace ChartTools
{
    public class Note<TLane> : Note where TLane : struct
    {
        public TLane Lane { get; set; }

        internal override byte NoteIndex => Convert.ToByte(Lane);

        public Note(TLane lane) => Lane = lane;
    }
}
