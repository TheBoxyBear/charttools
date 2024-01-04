using ChartTools.Events;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Configuration;

/// <summary>
/// Defines how duplicate track objects are handled.
/// </summary>
public enum DuplicateTrackObjectPolicy : byte
{
    /// <summary>
    /// Throw an exception with the position.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Include only the first object
    /// </summary>
    IncludeFirst,
    /// <summary>
    /// Include all objects.
    /// </summary>
    IncludeAll,
}

/// <summary>
/// Define where lyrics are obtained when writing a format that defines lyrics as events.
/// </summary>
public enum LyricEventSource : byte
{
    /// <summary>
    /// Obtain lyrics from <see cref="Song.GlobalEvents"/>.
    /// </summary>
    GlobalEvents,
    /// <summary>
    /// Obtain lyrics from the <see cref="InstrumentSet.Vocals"/> instrument.
    /// </summary>
    Vocals
}

/// <summary>
/// Defines how overlapping star power phrases should be handled.
/// </summary>
public enum OverlappingSpecialPhrasePolicy : byte
{
    /// <summary>
    /// Throw an exception.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Ignore the overlapping phrase.
    /// </summary>
    Ignore,
    /// <summary>
    /// Cut the length of the first phrase to the start of the next one.
    /// </summary>
    Cut,
}

/// <summary>
/// Defines how a tempo anchor with no parent marker is handled.
/// </summary>
public enum TempolessAnchorPolicy
{
    /// <summary>
    /// Throw an exception.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Ignore the anchor.
    /// </summary>
    Ignore,
    /// <summary>
    /// Create a tempo marker with the anchor.
    /// </summary>
    Create
}

/// <summary>
/// Defines how notes within ticks of each other are handled during a Midi operation.
/// </summary>
public enum SnappedNotesPolicy : byte
{
    /// <summary>
    /// Throw an exception.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Combine the notes as a single chord at the position of the earlier note.
    /// </summary>
    Snap,
    /// <summary>
    /// Leave each note as its own chord.
    /// </summary>
    Ignore
}

/// <summary>
/// Defines how <see cref="EventTypeHelper.Local.Solo"/> or <see cref="EventTypeHelper.Local.SoloEnd"/> events are handled when there are no star power phrases.
/// </summary>
/// <remarks><see cref="StoreAsEvents"/> is always used when star power phrases are present.</remarks>
public enum SoloNoStarPowerPolicy : byte
{
    /// <summary>
    /// Store the events under <see cref="Track.LocalEvents"/>.
    /// </summary>
    StoreAsEvents,
    /// <summary>
    /// Convert the space between the <see cref="EventTypeHelper.Local.Solo"/> and <see cref="EventTypeHelper.Local.SoloEnd"/> event to a star power phrase.
    /// </summary>
    Convert
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
/// Defines how unknown sections or Midi chunks are handled.
/// </summary>
public enum UnknownSectionPolicy : byte
{
    /// <summary>
    /// Throw an exception with the section or chunk header.
    /// </summary>
    ThrowException,
    /// <summary>
    /// Store the raw data to be included when writing.
    /// </summary>
    Store
}

/// <summary>
/// Defines chord modifiers not supported by the target format are handled.
/// </summary>
public enum UnsupportedModifierPolicy : byte
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
    IgnoreChord
}
