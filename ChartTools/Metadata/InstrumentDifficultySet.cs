using ChartTools.IO.Ini;

namespace ChartTools;

/// <summary>
/// Stores the estimated difficulties for instruments
/// </summary>
public class InstrumentDifficultySet
{
	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardLeadGuitar"/>, <see cref="InstrumentIdentity.StandardCoopGuitar"/> and <see cref="InstrumentIdentity.StandardRhythmGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.StandardGuitarDifficulty)]
	public sbyte? StandardGuitar
    {
        get => m_standardGuitar;
        set => m_standardGuitar = value;
    }
    private sbyte? m_standardGuitar;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.StandardBass"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.StandardBassDifficulty)]
	public sbyte? StandardBass
    {
        get => m_standardBass;
        set => m_standardBass = value;
    }
    private sbyte? m_standardBass;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.Drums"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.DrumsDifficulty)]
	public sbyte? Drums
    {
        get => m_drums;
        set => m_drums = value;
    }
    private sbyte? m_drums;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.StandardKeys"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.StandardKeysDifficulty)]
	public sbyte? StandardKeys
    {
        get => m_standardKeys;
        set => m_standardKeys = value;
    }
    private sbyte? m_standardKeys;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.GHLGuitarDifficulty)]
	public sbyte? GHLGuitar
    {
        get => m_ghlGuitar;
        set => m_ghlGuitar = value;
    }
    private sbyte? m_ghlGuitar;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLBass"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLBassDifficulty)]
	public sbyte? GHLBass
    {
        get => m_ghlBass;
        set => m_ghlBass = value;
    }
    private sbyte? m_ghlBass;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLRhythmGuitar"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLRhythmGuitarDifficulty)]
    public sbyte? GHLRhythmGuitar
    {
        get => m_ghlRhythmGuitar;
        set => m_ghlRhythmGuitar = value;
    }
    private sbyte? m_ghlRhythmGuitar;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLCoopGuitar"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLCoopGuitarDifficulty)]
    public sbyte? GHLCoopGuitar
    {
        get => m_ghlCoopGuitar;
        set => m_ghlCoopGuitar = value;
    }
    private sbyte? m_ghlCoopGuitar;

    public ref sbyte? GetDifficulty(InstrumentIdentity identity)
    {
        switch (identity)
        {
            case InstrumentIdentity.StandardLeadGuitar:
            case InstrumentIdentity.StandardCoopGuitar:
            case InstrumentIdentity.StandardRhythmGuitar:
                return ref m_standardGuitar;
            case InstrumentIdentity.StandardBass:
                return ref m_standardBass;
            case InstrumentIdentity.Drums:
                return ref m_drums;
            case InstrumentIdentity.StandardKeys:
                return ref m_standardKeys;
            case InstrumentIdentity.GHLGuitar:
                return ref m_ghlGuitar;
            case InstrumentIdentity.GHLBass:
                return ref m_ghlBass;
            case InstrumentIdentity.GHLRhythmGuitar:
                return ref m_ghlRhythmGuitar;
            case InstrumentIdentity.GHLCoopGuitar:
                return ref m_ghlCoopGuitar;
            default:
                throw new UndefinedEnumException(identity);
        }
    }
}
