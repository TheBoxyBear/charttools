using ChartTools.IO.Ini;

using System.Reflection;

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
        get => _standardGuitar;
        set => _standardGuitar = value;
    }
    private sbyte? _standardGuitar;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.StandardBass"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.StandardBassDifficulty)]
	public sbyte? StandardBass
    {
        get => _standardBass;
        set => _standardBass = value;
    }
    private sbyte? _standardBass;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.Drums"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.DrumsDifficulty)]
	public sbyte? Drums
    {
        get => _drums;
        set => _drums = value;
    }
    private sbyte? _drums;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.StandardKeys"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.StandardKeysDifficulty)]
	public sbyte? StandardKeys
    {
        get => _standardKeys;
        set => _standardKeys = value;
    }
    private sbyte? _standardKeys;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.GHLGuitarDifficulty)]
	public sbyte? GHLGuitar
    {
        get => _ghlGuitar;
        set => _ghlGuitar = value;
    }
    private sbyte? _ghlGuitar;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLBass"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLBassDifficulty)]
	public sbyte? GHLBass
    {
        get => _ghlBass;
        set => _ghlBass = value;
    }
    private sbyte? _ghlBass;

	/// <summary>
	/// Difficulty of <see cref="InstrumentIdentity.GHLRhythmGuitar"/>
	/// </summary>
	[IniKeySerializable(IniFormatting.GHLRhythmGuitarDifficulty)]
	public sbyte? GHLRhythmGuitar
    {
        get => _ghlRhythmGuitar;
        set => _ghlRhythmGuitar = value;
    }
    private sbyte? _ghlRhythmGuitar;

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLCoopGuitar"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLCoopGuitarDifficulty)]
	public sbyte? GHLCoopGuitar
    {
        get => _ghlCoopGuitar;
        set => _ghlCoopGuitar = value;
    }
    private sbyte? _ghlCoopGuitar;

    /// <summary>
    /// Gets the difficulty for an <see cref="InstrumentIdentity"/>.
    /// </summary>
    public ref sbyte? GetDifficulty(InstrumentIdentity identity)
    {
        switch (identity)
        {
            case InstrumentIdentity.StandardLeadGuitar:
            case InstrumentIdentity.StandardCoopGuitar:
            case InstrumentIdentity.StandardRhythmGuitar:
                return ref _standardGuitar;
            case InstrumentIdentity.StandardBass:
                return ref _standardBass;
            case InstrumentIdentity.Drums:
                return ref _drums;
            case InstrumentIdentity.StandardKeys:
                return ref _standardKeys;
            case InstrumentIdentity.GHLGuitar:
                return ref _ghlGuitar;
            case InstrumentIdentity.GHLBass:
                return ref _ghlBass;
            case InstrumentIdentity.GHLRhythmGuitar:
                return ref _ghlRhythmGuitar;
            case InstrumentIdentity.GHLCoopGuitar:
                return ref _ghlCoopGuitar;
            default:
                throw new UndefinedEnumException(identity);
        }
    }
}
