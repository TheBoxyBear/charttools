namespace ChartTools;

public abstract class LaneNote : INote
{
    public abstract byte Index { get; }

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
