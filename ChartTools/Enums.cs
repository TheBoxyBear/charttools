namespace ChartTools;

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
public enum InstrumentIdentity : byte { Drums, GHLGuitar, GHLBass, LeadGuitar, RhythmGuitar, CoopGuitar, Bass, Keys }

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
