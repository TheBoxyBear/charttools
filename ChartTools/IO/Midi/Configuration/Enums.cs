namespace ChartTools.IO.Midi.Configuration;

public enum InvalidMidiEventPolicy : byte
{
    ThrowException,
    Ignore
}

public enum MisalignedBigRockMarkersPolicy : byte
{
    ThrowException,
    IgnoreAll,
    IncludeFirst,
    Combine
}

public enum MissingBigRockMarkerPolicy : byte
{
    ThrowException,
    IgnoreAll,
    IgnoreMissing
}

/// <summary>
/// Difficulty of the <see cref="Track"/> to serve as a source of for track objects for which the target format requires these objects to be the same across all difficulties.
/// </summary>
/// <remarks>Can be cast from <see cref="Difficulty"/>.</remarks>
public enum TrackObjectSource : byte
{
    /// <summary>
    /// Use the objects from the <see cref="Difficulty.Easy"/> track.
    /// </summary>
    Easy,
    /// <summary>
    /// Use the objects from the <see cref="Difficulty.Medium"/> track.
    /// </summary>
    Medium,
    /// <summary>
    /// Use the objects from the <see cref="Difficulty.Hard"/> track.
    /// </summary>
    Hard,
    /// <summary>
    /// Use the objects from the <see cref="Difficulty.Expert"/> track.
    /// </summary>
    Expert,
    /// <summary>
    /// Combine the unique track objects from all the tracks in the instrument.
    /// </summary>
    Merge,
}

/// <summary>
/// Defines how lead guitar and bass and handled when the Midi mapping is uncertain.
/// </summary>
public enum UncertainFormatPolicy : byte
{
    /// <summary>
    /// Throw an exception.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Use the format that was defaulted to when reading.
    /// </summary>
    /// <remarks>Policy is invalid when reading.</remarks>
    UseReadingDefault,
    /// <summary>
    /// Default to the Guitar Hero 2 format.
    /// </summary>
    UseGuitarHero2,
    /// <summary>
    /// Default to the Rock Band format.
    /// </summary>
    UseRockBand
}

/// <summary>
/// Defines chord modifiers not supported by the target format are handled.
/// </summary>
public enum UnsupportedModifiersPolicy : byte
{
    /// <summary>
    /// Throw an exception with the modifier index.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Convert the modifier to one supported by the format.
    /// </summary>
    /// <remarks>Will throw an exception if the modifier cannot be converted.</remarks>
    Convert,
    /// <summary>
    /// Ignore the modifier.
    /// </summary>
    IgnoreModifier,
    /// <summary>
    /// Ignore the chord containing the modifier.
    /// </summary>
    IgnoreChord,
}

[Flags]
public enum UnsupportedModifiersResults : byte
{
    None,
    Chord,
    Modifiers
}

/// <summary>
/// Defines how track object defined with a <see cref="NoteOffEvent"/> with no matching <see cref="NoteOnEvent"/> are handled.
/// </summary>
public enum UnopenedTrackObjectPolicy : byte
{
    /// <summary>
    /// Throw an exception with the event position and index.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Create a track object at the position of the closing event and a length of 0.
    /// </summary>
    Create,
    /// <summary>
    /// Ignore the event.
    /// </summary>
    Ignore
}

/// <summary>
/// Defines how track object defined with a <see cref="NoteOnEvent"/> with no matching <see cref="NoteOffEvent"/> are handled.
/// </summary>
public enum UnclosedTrackObjectPolicy : byte
{
    /// <summary>
    /// Throw an exception with the event position and index.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Include the track object with a length going up to the next track object opening of the same index.
    /// </summary>
    Include,
    /// <summary>
    /// Ignore the event and track object.
    /// </summary>
    Ignore
}
