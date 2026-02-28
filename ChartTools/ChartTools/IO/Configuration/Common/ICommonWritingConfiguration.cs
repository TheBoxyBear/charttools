namespace ChartTools.IO.Configuration.Common;

public interface ICommonWritingConfiguration
{
    /// <see cref="Configuration.UnsupportedModifiersPolicy"/>
    public UnsupportedModifiersPolicy UnsupportedModifiersPolicy { get; init; }

    internal static ChordDataFlags GetUnsupportedModifierChordFlags(uint position, UnsupportedModifiersPolicy policy) => policy switch
    {
        UnsupportedModifiersPolicy.ThrowException => throw new Exception($"Chord at position {position} as an unsupported modifier for the chart format."),
        UnsupportedModifiersPolicy.IgnoreChord    => ChordDataFlags.None,
        UnsupportedModifiersPolicy.IgnoreModifier => ChordDataFlags.Chord,
        UnsupportedModifiersPolicy.Convert        => ChordDataFlags.Chord | ChordDataFlags.Modifiers,
        _ => throw ConfigurationExceptions.UnsupportedPolicy(policy)
    };
}
