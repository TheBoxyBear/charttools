namespace ChartTools.IO.Components;

/// <summary>
/// Flag-based version of the <see cref="Difficulty"/> enum for use in <see cref="InstrumentComponentList"/>
/// </summary>
[Flags]
public enum DifficultySet : byte
{
	None   = 0,
	Easy   = 1 << 0,
	Medium = 1 << 1,
	Hard   = 1 << 2,
	Expert = 1 << 3,
	All    = Easy | Medium | Hard | Expert
};

/// <summary>
/// Provides extension methods to the <see cref="Difficulty"/> and <see cref="DifficultySet"/> enums.
/// </summary>
public static class DifficultyExtensions
{
	/// <summary>
	/// Converts a <see cref="Difficulty"/> value to a <see cref="DifficultySet"/>.
	/// </summary>
	/// <param name="difficulty">Difficulty value to convert</param>
	/// <returns><see cref="DifficultySet"/> where the matching flag of the original value is set</returns>
	public static DifficultySet ToSet(this Difficulty difficulty) => (DifficultySet)(1 << (int)difficulty);
}

public record InstrumentComponentList()
{
	public static InstrumentComponentList Full() => new()
	{
		Drums                = DifficultySet.All,
		StandardLeadGuitar   = DifficultySet.All,
		StandardCoopGuitar   = DifficultySet.All,
		StandardRhythmGuitar = DifficultySet.All,
		StandardBass         = DifficultySet.All,
		GHLGuitar            = DifficultySet.All,
		GHLBass              = DifficultySet.All,
		GHLRhythmGuitar      = DifficultySet.All,
		GHLCoopGuitar        = DifficultySet.All,
		StandardKeys         = DifficultySet.All,
	};

	// Manually defining backing field to return by reference
	public DifficultySet Drums
	{
		get => _drums;
		set => _drums = value;
	}
	private DifficultySet _drums;

	public DifficultySet StandardLeadGuitar
	{
		get => _standardLeadGuitar;
		set => _standardLeadGuitar = value;
	}
	private DifficultySet _standardLeadGuitar;

	public DifficultySet StandardCoopGuitar
	{
		get => _standardCoopGuitar;
		set => _standardCoopGuitar = value;
	}
	private DifficultySet _standardCoopGuitar;

	public DifficultySet StandardRhythmGuitar
	{
		get => _standardRhythmGuitar;
		set => _standardRhythmGuitar = value;
	}
	private DifficultySet _standardRhythmGuitar;

	public DifficultySet StandardBass
	{
		get => _standardBass;
		set => _standardBass = value;
	}
	private DifficultySet _standardBass;

	public DifficultySet StandardKeys
	{
		get => _standardKeys;
		set => _standardKeys = value;
	}
	private DifficultySet _standardKeys;

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

	public DifficultySet GHLRhythmGuitar
	{
		get => _ghlRhythmGuitar;
		set => _ghlRhythmGuitar = value;
	}
	private DifficultySet _ghlRhythmGuitar;

	public DifficultySet GHLCoopGuitar
	{
		get => _ghlCoopGuitar;
		set => _ghlCoopGuitar = value;
	}
	private DifficultySet _ghlCoopGuitar;

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
			case InstrumentIdentity.StandardLeadGuitar:
				return ref _standardLeadGuitar;
			case InstrumentIdentity.StandardCoopGuitar:
				return ref _standardCoopGuitar;
			case InstrumentIdentity.StandardRhythmGuitar:
				return ref _standardRhythmGuitar;
			case InstrumentIdentity.StandardBass:
				return ref _standardBass;
			case InstrumentIdentity.GHLGuitar:
				return ref _ghlGuitar;
			case InstrumentIdentity.GHLBass:
				return ref _ghlBass;
			case InstrumentIdentity.GHLRhythmGuitar:
				return ref _ghlRhythmGuitar;
			case InstrumentIdentity.GHLCoopGuitar:
				return ref _ghlCoopGuitar;
			case InstrumentIdentity.StandardKeys:
				return ref _standardKeys;
			default:
				throw new UndefinedEnumException(instrument);
		}
	}

	public ref DifficultySet Map(StandardInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return ref Map((InstrumentIdentity)instrument);
	}

	public ref DifficultySet Map(GHLInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return ref Map((InstrumentIdentity)instrument);
	}
}
