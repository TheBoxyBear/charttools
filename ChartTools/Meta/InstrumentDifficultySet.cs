using ChartTools.IO.Ini;

namespace ChartTools.Meta;

/// <summary>
/// Stores the estimated difficulties for instruments
/// </summary>
public class InstrumentDifficultySet
{
	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardLeadGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.StandardLeadGuitar)]
	public sbyte? StandardLeadGuitar
	{
		get => m_standardLeadGuitar;
		set => m_standardLeadGuitar = value;
	}
	private sbyte? m_standardLeadGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardRhythmGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.StandardRhythmGuitar)]
	public sbyte? StandardRhythmGuitar
	{
		get => m_standardRhythmGuitar;
		set => m_standardRhythmGuitar = value;
	}
	private sbyte? m_standardRhythmGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardCoopGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.StandardCoopGuitar)]
	public sbyte? StandardCoopGuitar
	{
		get => m_standardCoopGuitar;
		set => m_standardCoopGuitar = value;
	}
	private sbyte? m_standardCoopGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardBass"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.StandardBass)]
	public sbyte? StandardBass
	{
		get => m_standardBass;
		set => m_standardBass = value;
	}
	private sbyte? m_standardBass;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.Drums"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.Drums)]
	public sbyte? Drums
	{
		get => m_drums;
		set => m_drums = value;
	}
	private sbyte? m_drums;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardKeys"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.StandardKeys)]
	public sbyte? StandardKeys
	{
		get => m_standardKeys;
		set => m_standardKeys = value;
	}
	private sbyte? m_standardKeys;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLLeadGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.GHLLeadGuitar)]
	public sbyte? GHLLeadGuitar
	{
		get => m_ghlLeadGuitar;
		set => m_ghlLeadGuitar = value;
	}
	private sbyte? m_ghlLeadGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLRhythmGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.GHLRhythmGuitar)]
	public sbyte? GHLRhythmGuitar
	{
		get => m_ghlRhythmGuitar;
		set => m_ghlRhythmGuitar = value;
	}
	private sbyte? m_ghlRhythmGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLCoopGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.GHLCoopGuitar)]
	public sbyte? GHLCoopGuitar
	{
		get => m_ghlCoopGuitar;
		set => m_ghlCoopGuitar = value;
	}
	private sbyte? m_ghlCoopGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLBass"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.Difficulties.GHLBass)]
	public sbyte? GHLBass
	{
		get => m_ghlBass;
		set => m_ghlBass = value;
	}
	private sbyte? m_ghlBass;

	public ref sbyte? GetDifficulty(InstrumentIdentity identity)
	{
		switch (identity)
		{
			case InstrumentIdentity.StandardLeadGuitar:
				return ref m_standardLeadGuitar;
			case InstrumentIdentity.StandardRhythmGuitar:
				return ref m_standardRhythmGuitar;
			case InstrumentIdentity.StandardCoopGuitar:
				return ref m_standardCoopGuitar;
			case InstrumentIdentity.StandardBass:
				return ref m_standardBass;
			case InstrumentIdentity.Drums:
				return ref m_drums;
			case InstrumentIdentity.StandardKeys:
				return ref m_standardKeys;
			case InstrumentIdentity.GHLLeadGuitar:
				return ref m_ghlLeadGuitar;
			case InstrumentIdentity.GHLRhythmGuitar:
				return ref m_ghlRhythmGuitar;
			case InstrumentIdentity.GHLCoopGuitar:
				return ref m_ghlCoopGuitar;
			case InstrumentIdentity.GHLBass:
				return ref m_ghlBass;
			default:
				throw new UndefinedEnumException(identity);
		}
	}
}
