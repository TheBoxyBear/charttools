namespace ChartTools.IO.Components;

[Flags]
public enum DifficultySet : byte
{
    None = 0,
    Easy = 1 << 0,
    Medium = 1 << 1,
    Hard = 1 << 2,
    Expert = 1 << 3,
    All = Easy | Medium | Hard | Expert
};


public static class DifficultyExtensions
{
    public static DifficultySet ToSet(this Difficulty difficulty) => (DifficultySet)(1 << (int)difficulty);
}

public record InstrumentComponentList()
{
    public static InstrumentComponentList Full() => new()
    {
        Drums       = DifficultySet.All,
        LeadGuitar  = DifficultySet.All,
        CoopGuitar  = DifficultySet.All,
        RythmGuitar = DifficultySet.All,
        Bass        = DifficultySet.All,
        GHLGuitar   = DifficultySet.All,
        GHLBass     = DifficultySet.All,
        Keys        = DifficultySet.All,
        Vocals      = DifficultySet.All
    };

    public DifficultySet Drums
    {
        get => _drums;
        set => _drums = value;
    }
    private DifficultySet _drums;

    public DifficultySet LeadGuitar
    {
        get => _leadGuitar;
        set => _leadGuitar = value;
    }
    private DifficultySet _leadGuitar;

    public DifficultySet CoopGuitar
    {
        get => _coopGuitar;
        set => _coopGuitar = value;
    }
    private DifficultySet _coopGuitar;

    public DifficultySet RythmGuitar
    {
        get => _rythmGuitar;
        set => _rythmGuitar = value;
    }
    private DifficultySet _rythmGuitar;

    public DifficultySet Bass
    {
        get => _bass;
        set => _bass = value;
    }
    private DifficultySet _bass;

    public DifficultySet GHLGuitar
    {
        get => _ghlGuitar;
        set => _ghlGuitar = value;
    }
    private DifficultySet _ghlGuitar;

    public DifficultySet GHLBass
    {
        get => _ghlBass;
        set => _ghlBass = value;
    }
    private DifficultySet _ghlBass;

    public DifficultySet Keys
    {
        get => _keys;
        set => _keys = value;
    }
    private DifficultySet _keys;

    public DifficultySet Vocals
    {
        get => _vocals;
        set => _vocals = value;
    }
    private DifficultySet _vocals;

    public InstrumentComponentList(InstrumentIdentity identity, DifficultySet difficulties = DifficultySet.All) : this()
    {
        Validator.ValidateEnum(identity);
        Validator.ValidateEnum(difficulties);

        Map(identity) = difficulties;
    }

    public InstrumentComponentList(StandardInstrumentIdentity identity, DifficultySet difficulties = DifficultySet.All) : this()
    {
        Validator.ValidateEnum(identity);
        Validator.ValidateEnum(difficulties);

        Map((InstrumentIdentity)identity) = difficulties;
    }

    public InstrumentComponentList(GHLInstrumentIdentity identity, DifficultySet difficulties = DifficultySet.All) : this()
    {
        Validator.ValidateEnum(identity);
        Validator.ValidateEnum(difficulties);

        Map((InstrumentIdentity)identity) = difficulties;
    }

    public ref DifficultySet Map(InstrumentIdentity instrument)
    {
        switch (instrument)
        {
            case InstrumentIdentity.Drums:
                return ref _drums;
            case InstrumentIdentity.LeadGuitar:
                return ref _leadGuitar;
            case InstrumentIdentity.CoopGuitar:
                return ref _coopGuitar;
            case InstrumentIdentity.RhythmGuitar:
                return ref _rythmGuitar;
            case InstrumentIdentity.Bass:
                return ref _bass;
            case InstrumentIdentity.GHLGuitar:
                return ref _ghlGuitar;
            case InstrumentIdentity.GHLBass:
                return ref _ghlBass;
            case InstrumentIdentity.Keys:
                return ref _keys;
            case InstrumentIdentity.Vocals:
                return ref _vocals;
            default:
                throw new UndefinedEnumException(instrument);
        }
    }

    public ref DifficultySet Map(StandardInstrumentIdentity instrument)
    {
        Validator.ValidateEnum(instrument);
        return ref Map((InstrumentIdentity)(instrument));
    }

    public ref DifficultySet Map(GHLInstrumentIdentity instrument)
    {
        Validator.ValidateEnum(instrument);
        return ref Map((InstrumentIdentity)(instrument));
    }
}
