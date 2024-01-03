namespace ChartTools
{
    /// <summary>
    /// Difficulty levels
    /// </summary>
    public enum Difficulty : byte
    {
        /// <summary>
        /// Easy difficulty
        /// </summary>
        Easy,
        /// <summary>
        /// Medium difficulty
        /// </summary>
        Medium,
        /// <summary>
        /// Hard difficulty
        /// </summary>
        Hard,
        /// <summary>
        /// Expert difficulty
        /// </summary>
        Expert
    }
    /// <summary>
    /// Modifier that affects the way the chord can be played
    /// </summary>
    [Flags]
    public enum DrumsChordModifiers : byte
    {
        /// <inheritdoc cref="StandardChordModifiers.None"/>
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
        /// <summary>
        /// Kick note, shown as a purple line
        /// </summary>
        Kick,
        /// <summary>
        /// Red pad
        /// </summary>
        Red,
        /// <summary>
        /// Yellow pad
        /// </summary>
        Yellow,
        /// <summary>
        /// Blue pad
        /// </summary>
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
        /// <remarks>In Clone Hero, double kicks are enabled with the "2x Kick" modifier and are not limited to a single difficulty.</remarks>
        DoubleKick
    }
    public enum FileType : byte { Chart, Ini, MIDI }
    /// <summary>
    /// Modifier that affects how a <see cref="GHLChord"/> can be played
    /// </summary>
    [Flags]
    public enum GHLChordModifiers : byte
    {
        /// <inheritdoc cref="StandardChordModifiers.None"/>
        None = 0,
        /// <inheritdoc cref="StandardChordModifiers.ExplicitHopo"/>
        ExplicitHopo = 1,
        /// <inheritdoc cref="StandardChordModifiers.HopoInvert"/>
        HopoInvert = 2,
        /// <inheritdoc cref="StandardChordModifiers.Tap"/>
        Tap = 4
    }
    /// <summary>
    /// Guitar Hero Live instruments
    /// </summary>
    /// <remarks>Casting to <see cref="InstrumentIdentity"/> will match the instrument.</remarks>
    public enum GHLInstrumentIdentity : byte { Guitar = 1, Bass }
    /// <summary>
    /// Frets for a GHL note
    /// </summary>
    public enum GHLLane : byte { Open, Black1, Black2, Black3, White1, White2, White3 }

    /// <summary>
    /// Origins of an instrument
    /// </summary>
    public enum MidiInstrumentOrigin : byte
    {
        NA,
        Unknown,
        GuitarHero1,
        GuitarHero2 = 4,
        GuitarHero2Uncertain = Unknown | GuitarHero2,
        RockBand = 6,
        RockBandUncertain = Unknown | RockBand,
    }

    /// <summary>
    /// All instruments
    /// </summary>
    public enum InstrumentIdentity : byte { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys, Vocals }
    public enum InstrumentType : byte { Drums, GHL, Standard, Vocals }

    /// <summary>
    /// Modifier that affects how a <see cref="StandardChord"/> can be played
    /// </summary>
    /// <remarks></remarks>
    [Flags]
    public enum StandardChordModifiers : byte
    {
        /// <summary>
        /// No modifier
        /// </summary>
        None = 0,
        /// <summary>
        /// The Hopo state is not relative to the previous chord.
        /// </summary>
        ExplicitHopo = 1,
        /// <summary>
        /// Forced Hopo if <see cref="ExplicitHopo"/> is set, otherwise inverts the natural state relative to the previous chord
        /// </summary>
        HopoInvert = 2,
        ForcedHopo = ExplicitHopo | HopoInvert,
        ForcedStrum = ExplicitHopo,
        /// <summary>
        /// The chord can be played without strumming
        /// </summary>
        Tap = 4,
        Big = 8
    }
    /// <summary>
    /// Standard five-fret instruments
    /// </summary>
    /// <remarks><inheritdoc cref="GHLInstrumentIdentity"/></remarks>
    public enum StandardInstrumentIdentity : byte { LeadGuitar = 3, RhythmGuitar, CoopGuitar, Bass, Keys }
    /// <summary>
    /// Frets for a standard note
    /// </summary>
    public enum StandardLane : byte { Open, Green, Red, Yellow, Blue, Orange }
    /// <summary>
    /// Types of <see cref="TrackSpecialPhrase"/>
    /// </summary>
    public enum TrackSpecialPhraseType : byte
    {
        /// <summary>
        /// The <see cref="TrackSpecialPhrase.Type"/> is not a recognized phrase type
        /// </summary>
        Unknown,
        /// <summary>
        /// Grants star power if all notes are hit
        /// </summary>
        StarPowerGain,
        /// <summary>
        /// Allows the activation of star power
        /// </summary>
        StarPowerActivation,
        Player1FaceOff,
        Player2FaceOff,
        Trill,
        Tremolo,
        DrumsRoll = 65,
        DrumsDoubleRoll = 66
    }
    /// <summary>
    /// Types of <see cref="InstrumentSpecialPhrase"/>
    /// </summary>
    public enum InstrumentSpecialPhraseType : byte
    {
        Unknown,
        BigRockEnding
    }
}

namespace ChartTools.Lyrics
{
    /// <summary>
    /// Pitch values for <see cref="VocalsPitch"/>
    /// </summary>
    public enum VocalPitchValue : byte
    {
        /// <summary>
        /// No pitch
        /// </summary>
        None = 0,
        /// <summary>
        /// Second C (lowest pitch)
        /// </summary>
        C2 = 0x20 | VocalsKey.C,
        /// <summary>
        /// Second C#
        /// </summary>
        CSharp2 = 0x20 | VocalsKey.CSharp,
        /// <summary>
        /// Second D
        /// </summary>
        D2 = 0x20 | VocalsKey.D,
        /// <summary>
        /// Second E-flat
        /// </summary>
        Eb2 = 0x20 | VocalsKey.Eb,
        /// <summary>
        /// Second E
        /// </summary>
        E2 = 0x20 | VocalsKey.E,
        /// <summary>
        /// Second F
        /// </summary>
        F2 = 0x20 | VocalsKey.F,
        /// <summary>
        /// Second F#
        /// </summary>
        FSharp2 = 0x20 | VocalsKey.FSharp,
        /// <summary>
        /// Second G
        /// </summary>
        G2 = 0x20 | VocalsKey.G,
        /// <summary>
        /// Second G#
        /// </summary>
        GSharp2 = 0x20 | VocalsKey.GSharp,
        /// <summary>
        /// Second A
        /// </summary>
        A2 = 0x20 | VocalsKey.A,
        /// <summary>
        /// Second B-flat
        /// </summary>
        Bb2 = 0x20 | VocalsKey.Bb,
        /// <summary>
        /// Second B
        /// </summary>
        B2 = 0x20 | VocalsKey.B,
        /// <summary>
        /// Third C
        /// </summary>
        C3 = 0x30 | VocalsKey.C,
        /// <summary>
        /// Third C#
        /// </summary>
        CSharp3 = 0x30 | VocalsKey.CSharp,
        /// <summary>
        /// Third D
        /// </summary>
        D3 = 0x30 | VocalsKey.D,
        /// <summary>
        /// Third E-flat
        /// </summary>
        Eb3 = 0x30 | VocalsKey.Eb,
        /// <summary>
        /// Third E
        /// </summary>
        E3 = 0x30 | VocalsKey.E,
        /// <summary>
        /// Third F
        /// </summary>
        F3 = 0x30 | VocalsKey.F,
        /// <summary>
        /// Third F#
        /// </summary>
        FSharp3 = 0x30 | VocalsKey.FSharp,
        /// <summary>
        /// Third G
        /// </summary>
        G3 = 0x30 | VocalsKey.G,
        /// <summary>
        /// Third G#
        /// </summary>
        GSharp3 = 0x30 | VocalsKey.GSharp,
        /// <summary>
        /// Third A
        /// </summary>
        A3 = 0x30 | VocalsKey.A,
        /// <summary>
        /// Third B-flat
        /// </summary>
        Bb3 = 0x30 | VocalsKey.Bb,
        /// <summary>
        /// Third B
        /// </summary>
        B3 = 0x30 | VocalsKey.B,
        /// <summary>
        /// Third C
        /// </summary>
        C4 = 0x40 | VocalsKey.C,
        /// <summary>
        /// Fourth C#
        /// </summary>
        CSharp4 = 0x40 | VocalsKey.CSharp,
        /// <summary>
        /// Fourth D
        /// </summary>
        D4 = 0x40 | VocalsKey.D,
        /// <summary>
        /// Fourth E-flat
        /// </summary>
        Eb4 = 0x40 | VocalsKey.Eb,
        /// <summary>
        /// Fourth E
        /// </summary>
        E4 = 0x40 | VocalsKey.E,
        /// <summary>
        /// Fourth F
        /// </summary>
        F4 = 0x40 | VocalsKey.F,
        /// <summary>
        /// Fourth F#
        /// </summary>
        FSharp4 = 0x40 | VocalsKey.FSharp,
        /// <summary>
        /// Fourth G
        /// </summary>
        G4 = 0x40 | VocalsKey.G,
        /// <summary>
        /// Fourth G#
        /// </summary>
        GSharp4 = 0x40 | VocalsKey.GSharp,
        /// <summary>
        /// Fourth A
        /// </summary>
        A4 = 0x40 | VocalsKey.A,
        /// <summary>
        /// Fourth B-flat
        /// </summary>
        Bb4 = 0x40 | VocalsKey.Bb,
        /// <summary>
        /// Fourth B
        /// </summary>
        B4 = 0x40 | VocalsKey.B,
        /// <summary>
        /// Fifth
        /// </summary>
        C5 = 0x50 | VocalsKey.C,
        /// <summary>
        /// Fifth C#
        /// </summary>
        CSharp5 = 0x50 | VocalsKey.CSharp,
        /// <summary>
        /// Fifth D
        /// </summary>
        D5 = 0x50 | VocalsKey.D,
        /// <summary>
        /// Fifth E-flat
        /// </summary>
        Eb5 = 0x50 | VocalsKey.Eb,
        /// <summary>
        /// Fifth E
        /// </summary>
        E5 = 0x50 | VocalsKey.E,
        /// <summary>
        /// Fifth F
        /// </summary>
        F5 = 0x50 | VocalsKey.F,
        /// <summary>
        /// Fifth F#
        /// </summary>
        FSharp5 = 0x50 | VocalsKey.FSharp,
        /// <summary>
        /// Fifth G
        /// </summary>
        G5 = 0x50 | VocalsKey.G,
        /// <summary>
        /// Fifth G#
        /// </summary>
        GSharp5 = 0x50 | VocalsKey.GSharp,
        /// <summary>
        /// Fifth A
        /// </summary>
        A5 = 0x50 | VocalsKey.A,
        /// <summary>
        /// Fifth B-flat
        /// </summary>
        Bb5 = 0x50 | VocalsKey.Bb,
        /// <summary>
        /// Fifth B
        /// </summary>
        B5 = 0x50 | VocalsKey.B,
        /// <summary>
        /// Sixth C (highest pitch)
        /// </summary>
        C6 = 0x60 | VocalsKey.C
    }

    /// <summary>
    /// Keys making up <see cref="VocalPitchValue"/> without the octave
    /// </summary>
    public enum VocalsKey : byte
    {
        /// <summary>
        /// C key (Do)
        /// </summary>
        C,
        /// <summary>
        /// C# key
        /// </summary>
        CSharp,
        /// <summary>
        /// D key (Ré)
        /// </summary>
        D,
        /// <summary>
        /// E-flat key
        /// </summary>
        Eb,
        /// <summary>
        /// E key (Mi)
        /// </summary>
        E,
        /// <summary>
        /// F key (Fa)
        /// </summary>
        F,
        /// <summary>
        /// F# key
        /// </summary>
        FSharp,
        /// <summary>
        /// G key (Sol)
        /// </summary>
        G,
        /// <summary>
        /// G# key
        /// </summary>
        GSharp,
        /// <summary>
        /// A key (La)
        /// </summary>
        A,
        /// <summary>
        /// B-flat key
        /// </summary>
        Bb,
        /// <summary>
        /// B key (Si)
        /// </summary>
        B
    }
}
