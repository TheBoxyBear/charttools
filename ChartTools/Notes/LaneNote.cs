using System.Runtime.CompilerServices;

namespace ChartTools;

public abstract class LaneNote : INote, ILongObject
{
    /// <inheritdoc cref="INote.Index"/>
    public abstract byte Index { get; set; }

    /// <summary>
    /// Maximum length the note can be held for extra points
    /// </summary>
    public uint Sustain { get; set; }

    uint ILongObject.Length
    {
        get => Sustain;
        set => Sustain = value;
    }
}

public class LaneNote<TLane>(TLane lane) : LaneNote
    where TLane : struct, Enum
{
    public LaneNote() : this(default) { }

    public TLane Lane
    {
        get => lane;
        set => lane = value;
    }

    /// <inheritdoc cref="INote.Index"/>
    public override byte Index
    {
        get => Unsafe.As<TLane, byte>(ref lane);
        set => lane = Unsafe.As<byte, TLane>(ref value);
    }
}
