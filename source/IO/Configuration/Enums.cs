using ChartTools.Events;
using ChartTools.Lyrics;
using Melanchall.DryWetMidi.Core;

using System;

namespace ChartTools.IO.Configuration
{
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
    /// Defines how overlapping star power phrases should be handled
    /// </summary>
    public enum OverlappingSpecialPhrasePolicy : byte
    {
        ThrowException,
        Ignore,
        /// <summary>
        /// The length of the phrase is cut short to the start of the next one
        /// </summary>
        Cut,
    }
    /// <summary>
    /// Defines how a tempo anchor with no parent marker is handled
    /// </summary>
    public enum TempolessAnchorPolicy
    {
        ThrowException,
        Ignore,
        Create
    }
    /// <summary>
    /// Defines how notes within ticks of each other are handled during a Midi operation
    /// </summary>
    public enum SnappedNotesPolicy : byte
    {
        ThrowException,
        /// <summary>
        /// Notes are combines as a single chord at the position of the earlier note
        /// </summary>
        Snap,
        /// <summary>
        /// Each note is left as its own chord
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
        [Obsolete]
        Seperate
    }
    /// <summary>
    /// Defines how lead guitar and bass and handled when the Midi mapping is uncertain.
    /// </summary>
    public enum UncertainGuitarBassFormatPolicy : byte
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
        IgnoreChord,
    }
    /// <summary>
    /// Defines how track object defined with a <see cref="NoteOffEvent"/> with no matching <see cref="NoteOnEvent"/> are handled when reading Midi.
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
    /// Defines how track object defined with a <see cref="NoteOnEvent"/> with no matching <see cref="NoteOffEvent"/> are handled when reading Midi.
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
}
