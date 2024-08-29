namespace ChartTools.IO.Components;

public record ComponentList()
{
    public static ComponentList Global() => new()
    {
        Metadata     = true,
        SyncTrack    = true,
        GlobalEvents = true
    };

    public static ComponentList Full() => new()
    {
        Metadata     = true,
        SyncTrack    = true,
        GlobalEvents = true,
        Instruments  = InstrumentComponentList.Full()
    };

    public bool Metadata { get; set; }
    public bool SyncTrack { get; set; }
    public bool GlobalEvents { get; set; }

    public InstrumentComponentList Instruments { get; set; } = new();
}
