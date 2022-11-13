using System.Runtime.CompilerServices;

namespace ChartTools;

public abstract class LaneChord<TNote, TLane, TModifiers> : LaneChord, IChord
    where TNote : LaneNote<TLane>, new()
    where TLane : struct, Enum
    where TModifiers : struct, Enum
{
    /// <summary>
    /// Notes in the chord
    /// </summary>
    public new LaneNoteCollection<TNote, TLane> Notes { get; }

        public TModifiers Modifiers { get; set; }
        protected abstract TModifiers DefaultModifiers { get; }

    public LaneChord() : base() => Notes = new(OpenExclusivity);
    public LaneChord(uint position) : base(position) => Notes = new(OpenExclusivity);

    public override LaneNote CreateNote(byte index, uint sustain = 0)
    {
        var note = new TNote()
        {
            Lane = Unsafe.As<byte, TLane>(ref index),
            Sustain = sustain
        };

        Notes.Add(note);
        return note;
    }
}
