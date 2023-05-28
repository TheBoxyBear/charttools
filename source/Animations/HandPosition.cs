namespace ChartTools.Animations;

public class StandardInstrumentHandPosition : ITrackObject
{
    public uint Position { get; set; }
    public byte Index { get; }

    public StandardInstrumentHandPosition(uint position, byte index)
    {
        Position = position;
        Index = index;
    }
}
