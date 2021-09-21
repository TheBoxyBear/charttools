using ChartTools.Lyrics;
using System;

namespace ChartTools
{
    /// <summary>
    /// Difficulty levels
    /// </summary>
    public enum Difficulty : byte { Easy, Medium, Hard, Expert }
    /// <summary>
    /// Modifier that affects the way the chord can be played
    /// </summary>
    [Flags]
    public enum DrumsChordModifier : byte
    {
        None,
        /// <summary>
        /// *Unsupported*
        /// </summary>
        Accent,
        /// <summary>
        /// *Unsupported*
        /// </summary>
        Ghost,
        Flam = 4
    }
    /// <summary>
    /// Drums pads and pedals for a <see cref="DrumsNote"/>
    /// </summary>
    public enum DrumsLane : byte
    {
        Kick,
        Red,
        Yellow,
        Blue,
        /// <summary>
        /// Green when playing with four pads, orange when playing with five pads
        /// </summary>
        Green4Lane_Orange5Lane,
        /// <summary>
        /// Green when playing with five pad, otherwise converted to <see cref="Green4Lane_Orange5Lane"/>
        /// </summary>
        Green5Lane,
        /// <summary>
        /// <see cref="Kick"/> that only appears when playing with multiple pedals
        /// </summary>
        DoubleKick
    }
    /// <summary>
    /// Modifier that affects how a <see cref="GHLChord"/> can be played
    /// </summary>
    [Flags] public enum GHLChordModifier : byte
    {
        Natural,
        Invert,
        ForceStrum,
        ForceHopo,
        Tap
    }
    /// <summary>
    /// Guitar Hero Live instruments
    /// </summary>
    /// <remarks>Casting to <see cref="Instruments"/> will match the instrument.</remarks>
    public enum GHLInstrument : byte { Guitar = 1, Bass }
    /// <summary>
    /// Frets for a <see cref="GHLNote"/>
    /// </summary>
    public enum GHLLane : byte { Open, Black1, Black2, Black3, White1, White2, White3 }
    /// <summary>
    /// Types of <see cref="GlobalEvent"/>
    /// </summary>
    public enum GlobalEventType : byte
    {
        /// <summary>
        /// The backing <see cref="Event.EventTypeString"/> property does not match a known event type
        /// </summary>
        Unknown,
        /// <summary>
        /// Marks the start of a new <see cref="Phrase"/>
        /// </summary>
        PhraseStart,
        /// <summary>
        /// Marks the end of the current <see cref="Phrase"/>
        /// </summary>
        PhraseEnd,
        /// <summary>
        /// Marks a <see cref="Syllable"/> in the current <see cref="Phrase"/>
        /// </summary>
        Lyric,
        Idle,
        Play,
        HalfTempo,
        NormalTempo,
        Verse,
        Chorus,
        End,
        MusicStart,
        Lighting,
        CrowdLightersFast,
        CrowdLightersOff,
        CrowdLightersSlow,
        CrowdHalfTempo,
        CrowdNormalTempo,
        CrowdDoubleTempo,
        BandJump,
        /// <summary>
        /// Marks a new section used by Practice mode and in post-game summary
        /// </summary>
        Section,
        SyncHeadBang,
        SyncWag
    }
    /// <summary>
    /// Lighting effect caused by a <see cref="GlobalEvent"/> of type <see cref="GlobalEventType.Lighting"/>
    /// </summary>
    public enum LightingEffect
    {
        /// <summary>
        /// The backing argument of the event does not match a known lighting effect
        /// </summary>
        Unknwon,
        Flare,
        Blackout,
        Chase,
        Strobe,
        Color1,
        Color2,
        Sweep
    }
    /// <summary>
    /// Types of <see cref="LocalEvent"/>
    /// </summary>
    public enum LocalEventType : byte
    {
        /// <summary>
        /// The backing <see cref="Event.EventTypeString"/> property does not match a known event type
        /// </summary>
        Unknown,
        /// <summary>
        /// Marks the start of a Rock Band style solo section
        /// </summary>
        Solo,
        /// <summary>
        /// Marks the end of a Rock Band style solo section
        /// </summary>
        SoloEnd,
        GHL6,
        GHL6Forced,
        SoloOn,
        SoloOff,
        WailOn,
        WailOff,
        OwFaceOn,
        OwFaceOff
    }
    /// <summary>
    /// Defines how section global events are written
    /// </summary>
    public enum RockBandSectionFormat : byte { RockBand2, RockBand3 }
    /// <summary>
    /// Modifier that affects how a <see cref="StandardChord"/> can be played
    /// </summary>
    [Flags] public enum StandardChordModifier : byte
    {
        Natural,
        Invert,
        ForceStrum,
        ForceHopo,
        Tap
    }
    /// <summary>
    /// Standard five-fret instruments
    /// </summary>
    /// <remarks><inheritdoc cref="GHLInstrument"/></remarks>
    public enum StandardInstrument : byte { LeadGuitar = 3, RhythmGuitar, CoopGuitar, Bass, Keys }
    /// <summary>
    /// Frets for a <see cref="StandardNote"/>
    /// </summary>
    public enum StandardLane : byte { Open, Green, Red, Yellow, Blue, Orange }
    /// <summary>
    /// All instruments
    /// </summary>
    public enum Instruments : byte { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }
    public enum FileFormat : byte { Chart, Ini, MIDI }
}
