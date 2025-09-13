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

/// <summary>
/// Set of instruments and tracks to include in a read/write operation
/// </summary>
public record InstrumentComponentList()
{
	/// <summary>
	/// Creates a new <see cref="InstrumentComponentList"/> with all instruments and tracks included.
	/// </summary>
	public static InstrumentComponentList Full() => new()
	{
		Drums                = DifficultySet.All,
		StandardLeadGuitar   = DifficultySet.All,
		StandardCoopGuitar   = DifficultySet.All,
		StandardRhythmGuitar = DifficultySet.All,
		StandardBass         = DifficultySet.All,
		GHLLeadGuitar        = DifficultySet.All,
		GHLRhythmGuitar      = DifficultySet.All,
		GHLCoopGuitar        = DifficultySet.All,
		GHLBass              = DifficultySet.All,
		StandardKeys         = DifficultySet.All,
	};

	#region Instruments
	// Manually defining backing field to return by reference

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.Drums"/>
	/// </summary>
	public DifficultySet Drums
	{
		get => m_drums;
		set => m_drums = value;
	}
	private DifficultySet m_drums;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.StandardLeadGuitar"/>
	/// </summary>
	public DifficultySet StandardLeadGuitar
	{
		get => m_standardLeadGuitar;
		set => m_standardLeadGuitar = value;
	}
	private DifficultySet m_standardLeadGuitar;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.StandardCoopGuitar"/>
	/// </summary>
	public DifficultySet StandardCoopGuitar
	{
		get => m_standardCoopGuitar;
		set => m_standardCoopGuitar = value;
	}
	private DifficultySet m_standardCoopGuitar;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.StandardRhythmGuitar"/>
	/// </summary>
	public DifficultySet StandardRhythmGuitar
	{
		get => m_standardRhythmGuitar;
		set => m_standardRhythmGuitar = value;
	}
	private DifficultySet m_standardRhythmGuitar;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.StandardBass"/>
	/// </summary>
	public DifficultySet StandardBass
	{
		get => m_standardBass;
		set => m_standardBass = value;
	}
	private DifficultySet m_standardBass;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.StandardKeys"/>
	/// </summary>
	public DifficultySet StandardKeys
	{
		get => m_standardKeys;
		set => m_standardKeys = value;
	}
	private DifficultySet m_standardKeys;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.GHLLeadGuitar"/>
	/// </summary>
	public DifficultySet GHLLeadGuitar
	{
		get => m_ghlLeadGuitar;
		set => m_ghlLeadGuitar = value;
	}
	private DifficultySet m_ghlLeadGuitar;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.GHLRhythmGuitar"/>
	/// </summary>
	public DifficultySet GHLRhythmGuitar
	{
		get => m_ghlRhythmGuitar;
		set => m_ghlRhythmGuitar = value;
	}
	private DifficultySet m_ghlRhythmGuitar;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.GHLCoopGuitar"/>
	/// </summary>
	public DifficultySet GHLCoopGuitar
	{
		get => m_ghlCoopGuitar;
		set => m_ghlCoopGuitar = value;
	}
	private DifficultySet m_ghlCoopGuitar;

	/// <summary>
	/// Tracks to include for <see cref="InstrumentSet.GHLBass"/>
	/// </summary>
	public DifficultySet GHLBass
	{
		get => m_ghlBass;
		set => m_ghlBass = value;
	}
	private DifficultySet m_ghlBass;
	#endregion

	/// <summary>
	/// Creates a new <see cref="InstrumentComponentList"/> for a single instrument with the specified difficulties.
	/// </summary>
	/// <param name="identity">Instrument to include</param>
	/// <param name="difficulties">Tracks to include for the instrument</param>
	/// <exception cref="UndefinedEnumException"/>
	public InstrumentComponentList(InstrumentIdentity identity, DifficultySet difficulties = DifficultySet.All) : this()
	{
		Validator.ValidateEnum(identity);
		Validator.ValidateEnum(difficulties);

		Map(identity) = difficulties;
	}

	/// <summary>
	/// Creates a new <see cref="InstrumentComponentList"/> for a single standard instrument with the specified difficulties.
	/// </summary>
	/// <param name="identity">Instrument to include</param>
	/// <param name="difficulties">Tracks to include for the instrument</param>
	/// <exception cref="UndefinedEnumException"/>
	public InstrumentComponentList(StandardInstrumentIdentity identity, DifficultySet difficulties = DifficultySet.All) : this()
	{
		Validator.ValidateEnum(identity);
		Validator.ValidateEnum(difficulties);

		Map((InstrumentIdentity)identity) = difficulties;
	}

	/// <summary>
	/// Creates a new <see cref="InstrumentComponentList"/> for a single Guitar Hero Live instrument with the specified difficulties.
	/// </summary>
	/// <param name="identity">Instrument to include</param>
	/// <param name="difficulties">Tracks to include for the instrument</param>
	/// <exception cref="UndefinedEnumException"/>
	public InstrumentComponentList(GHLInstrumentIdentity identity, DifficultySet difficulties = DifficultySet.All) : this()
	{
		Validator.ValidateEnum(identity);
		Validator.ValidateEnum(difficulties);

		Map((InstrumentIdentity)identity) = difficulties;
	}

	/// <summary>
	/// Maps a instrument to its included tracks by reference.
	/// </summary>
	/// <param name="instrument"></param>
	/// <returns></returns>
	/// <exception cref="UndefinedEnumException"></exception>
	public ref DifficultySet Map(InstrumentIdentity instrument)
	{
		switch (instrument)
		{
			case InstrumentIdentity.Drums:
				return ref m_drums;
			case InstrumentIdentity.StandardLeadGuitar:
				return ref m_standardLeadGuitar;
			case InstrumentIdentity.StandardCoopGuitar:
				return ref m_standardCoopGuitar;
			case InstrumentIdentity.StandardRhythmGuitar:
				return ref m_standardRhythmGuitar;
			case InstrumentIdentity.StandardBass:
				return ref m_standardBass;
			case InstrumentIdentity.GHLLeadGuitar:
				return ref m_ghlLeadGuitar;
			case InstrumentIdentity.GHLBass:
				return ref m_ghlBass;
			case InstrumentIdentity.GHLRhythmGuitar:
				return ref m_ghlRhythmGuitar;
			case InstrumentIdentity.GHLCoopGuitar:
				return ref m_ghlCoopGuitar;
			case InstrumentIdentity.StandardKeys:
				return ref m_standardKeys;
			default:
				throw new UndefinedEnumException(instrument);
		}
	}

	/// <summary>
	/// Maps a standard instrument to its included tracks by reference.
	/// </summary>
	/// <param name="instrument"></param>
	/// <exception cref="UndefinedEnumException"></exception>
	public ref DifficultySet Map(StandardInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return ref Map((InstrumentIdentity)instrument);
	}

	/// <summary>
	/// Maps a Guitar Hero Live instrument to its included tracks by reference.
	/// </summary>
	/// <param name="instrument"></param>
	/// <exception cref="UndefinedEnumException"></exception>
	public ref DifficultySet Map(GHLInstrumentIdentity instrument)
	{
		Validator.ValidateEnum(instrument);
		return ref Map((InstrumentIdentity)instrument);
	}
}
