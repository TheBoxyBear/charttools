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
    public sbyte? StandardGuitar { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.StandardBass"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.StandardBassDifficulty)]
    public sbyte? StandardBass { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.Drums"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.DrumsDifficulty)]
    public sbyte? Drums { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.StandardKeys"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.StandardKeysDifficulty)]
    public sbyte? StandardKeys { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLGuitar"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLGuitarDifficulty)]
    public sbyte? GHLGuitar { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLBass"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLBassDifficulty)]
    public sbyte? GHLBass { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLRhythmGuitar"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLRhythmGuitarDifficulty)]
    public sbyte? GHLRhythmGuitar { get; set; }

    /// <summary>
    /// Difficulty of <see cref="InstrumentIdentity.GHLCoopGuitar"/>
    /// </summary>
    [IniKeySerializable(IniFormatting.GHLCoopGuitarDifficulty)]
    public sbyte? GHLCoopGuitar { get; set; }

    /// <summary>
    /// Gets the difficulty for an <see cref="InstrumentIdentity"/>.
    /// </summary>
    public sbyte? GetDifficulty(InstrumentIdentity identity)
        => GetDifficultyProperty(identity, out var info) ? (sbyte?)info!.GetValue(this) : null;

    /// <summary>
    /// Sets the difficulty for an <see cref="InstrumentIdentity"/>.
    /// </summary>
    public void SetDifficulty(InstrumentIdentity identity, sbyte? difficulty)
    {
        if (GetDifficultyProperty(identity, out var info))
            info!.SetValue(this, difficulty);
    }

    private bool GetDifficultyProperty(InstrumentIdentity identity, out PropertyInfo? info)
    {
        Validator.ValidateEnum(identity);
        var propName = identity switch
        {
            InstrumentIdentity.StandardLeadGuitar or InstrumentIdentity.StandardCoopGuitar or InstrumentIdentity.StandardRhythmGuitar => nameof(StandardGuitar),
            InstrumentIdentity.StandardBass => nameof(StandardBass),
            InstrumentIdentity.Drums => nameof(Drums),
            InstrumentIdentity.StandardKeys => nameof(StandardKeys),
            InstrumentIdentity.GHLGuitar => nameof(GHLGuitar),
            InstrumentIdentity.GHLBass => nameof(GHLBass),
            _ => null
        };

        if (propName is null)
        {
            info = null;
            return false;
        }

        info = typeof(InstrumentDifficultySet).GetProperty(propName);

        return true;
    }
}
