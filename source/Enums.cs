﻿using ChartTools.Lyrics;
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

    public enum VocalsPitches : byte
    {
        C2 = 0x20 | VocalsKey.C,
        CSharp2 = 0x20 | VocalsKey.CSharp,
        D2 = 0x20 | VocalsKey.D,
        Eb2 = 0x20 | VocalsKey.Eb,
        E2 = 0x20 | VocalsKey.E,
        F2 = 0x20 | VocalsKey.F,
        FSharp2 = 0x20 | VocalsKey.FSharp,
        G2 = 0x20 | VocalsKey.G,
        GSharp2 = 0x20 | VocalsKey.GSharp,
        A2 = 0x20 | VocalsKey.A,
        Bb2 = 0x20 | VocalsKey.Bb,
        B2 = 0x20 | VocalsKey.B,
        C3 = 0x30 | VocalsKey.C,
        CSharp3 = 0x30 | VocalsKey.CSharp,
        D3 = 0x30 | VocalsKey.D,
        Eb3 = 0x30 | VocalsKey.Eb,
        E3 = 0x30 | VocalsKey.E,
        F3 = 0x30 | VocalsKey.F,
        FSharp3 = 0x30 | VocalsKey.FSharp,
        G3 = 0x30 | VocalsKey.G,
        GSharp3 = 0x30 | VocalsKey.GSharp,
        A3 = 0x30 | VocalsKey.A,
        Bb3 = 0x30 | VocalsKey.Bb,
        B3 = 0x30 | VocalsKey.B,
        C4 = 0x40 | VocalsKey.C,
        CSharp4 = 0x40 | VocalsKey.CSharp,
        D4 = 0x40 | VocalsKey.D,
        Eb4 = 0x40 | VocalsKey.Eb,
        E4 = 0x40 | VocalsKey.E,
        F4 = 0x40 | VocalsKey.F,
        FSharp4 = 0x40 | VocalsKey.FSharp,
        G4 = 0x40 | VocalsKey.G,
        GSharp4 = 0x40 | VocalsKey.GSharp,
        A4 = 0x40 | VocalsKey.A,
        Bb4 = 0x40 | VocalsKey.Bb,
        B4 = 0x40 | VocalsKey.B,
        C5 = 0x50 | VocalsKey.C,
        CSharp5 = 0x50 | VocalsKey.CSharp,
        D5 = 0x50 | VocalsKey.D,
        Eb5 = 0x50 | VocalsKey.Eb,
        E5 = 0x50 | VocalsKey.E,
        F5 = 0x50 | VocalsKey.F,
        FSharp5 = 0x50 | VocalsKey.FSharp,
        G5 = 0x50 | VocalsKey.G,
        GSharp5 = 0x50 | VocalsKey.GSharp,
        A5 = 0x50 | VocalsKey.A,
        Bb5 = 0x50 | VocalsKey.Bb,
        B5 = 0x50 | VocalsKey.B,
        C6 = 0x60 | VocalsKey.C
    }
    public enum VocalsKey : byte
    {
        C,
        CSharp,
        D,
        Eb,
        E,
        F,
        FSharp,
        G,
        GSharp,
        A,
        Bb,
        B
    }
    /// <summary>
    /// All instruments
    /// </summary>
    public enum Instruments : byte { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys, Vocals }
    public enum FileFormat : byte { Chart, Ini, MIDI }
}
