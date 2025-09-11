namespace ChartTools;

/// <summary>
/// Difficulty levels from <see cref="Easy"/> to <see cref="Expert"/>
/// </summary>
/// <remarks>For <see cref="Drums"/>, the Expert+ track is defined by the presence of <see cref="DrumsLane.DoubleKick"/> notes on the <see cref="Expert"/> track.</remarks>
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
	ExplicitHopo = 1 << 0,
	/// <inheritdoc cref="StandardChordModifiers.HopoInvert"/>
	HopoInvert = 1 << 1,
	/// <inheritdoc cref="StandardChordModifiers.ForcedHopo"/>
	ForcedHopo = ExplicitHopo | HopoInvert,
	/// <inheritdoc cref="StandardChordModifiers.ForcedStrum"/>
	ForcedStrum = ExplicitHopo,
	/// <inheritdoc cref="StandardChordModifiers.Tap"/>
	Tap = 1 << 2,
}

/// <summary>
/// Guitar Hero Live instruments
/// </summary>
/// <remarks>Can be cast to <see cref="InstrumentIdentity"/>.</remarks>
public enum GHLInstrumentIdentity : byte
{
	/// <inheritdoc cref="InstrumentIdentity.GHLGuitar"/>
	LeadGuitar = InstrumentIdentity.GHLLeadGuitar,
	/// <inheritdoc cref="InstrumentIdentity.GHLBass"/>
	Bass = InstrumentIdentity.GHLBass,
	/// <inheritdoc cref="InstrumentIdentity.GHLRhythmGuitar"/>
	RhythmGuitar = InstrumentIdentity.GHLRhythmGuitar,
	/// <inheritdoc cref="InstrumentIdentity.GHLCoopGuitar"/>
	CoopGuitar = InstrumentIdentity.GHLCoopGuitar
}

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
public enum InstrumentIdentity : byte
{
	/// <summary>
	/// Four, five-lane or pro drums
	/// </summary>
	/// <remarks>Drum types have shared tracks with the type defined by the notes used</remarks>
	Drums,
	/// <summary>
	/// Six-lane Guitar Hero Live guitar
	/// </summary>
	GHLLeadGuitar,
	/// <summary>
	/// Six-lane Guitar Hero Live bass
	/// </summary>
	GHLBass,
	/// <summary>
	/// Six-lane Guitar Hero Live rhythm guitar
	/// </summary>
	GHLRhythmGuitar,
	/// <summary>
	/// Six-lane Guitar Hero Lice co-op guitar
	/// </summary>
	GHLCoopGuitar,
	/// <summary>
	/// Five-lane lead guitar
	/// </summary>
	/// <remarks>Primary instrument for most charts. Represents a combination of <see cref="StandardCoopGuitar"/> and <see cref="StandardRhythmGuitar"/> to be played by the same player.</remarks>
	StandardLeadGuitar,
	/// <summary>
	/// Five-lane rhythm guitar
	/// </summary>
	StandardRhythmGuitar,
	/// <summary>
	/// Five-lane co-op guitar
	/// </summary>
	/// <remarks>Isolates the lead part of <see cref="StandardLeadGuitar"/> for purpose of co-op play with a <see cref="StandardRhythmGuitar"/> player.</remarks>
	StandardCoopGuitar,
	/// <summary>
	/// Five-lane bass
	/// </summary>
	StandardBass,
	/// <summary>
	/// Five-lane synthesizer keyboard
	/// </summary>
	StandardKeys
}

/// <summary>
/// Types of instruments based on the notes contained
/// </summary>
public enum InstrumentType : byte
{
	/// <inheritdoc cref="InstrumentIdentity.Drums"/>
	Drums,
	/// <summary>
	/// Six-lane Guitar Hero Live instruments
	/// </summary>
	GHL,
	/// <summary>
	/// Five-lane instruments used by most games
	/// </summary>
	Standard,
}

/// <summary>
/// Modifiers that affects how a <see cref="StandardChord"/> can be played.
/// </summary>
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
	ExplicitHopo = 1 << 0,
	/// <summary>
	/// Forced Hopo if <see cref="ExplicitHopo"/> is set, otherwise inverts the natural state relative to the previous chord
	/// </summary>
	HopoInvert = 1 << 1,
	ForcedHopo = ExplicitHopo | HopoInvert,
	ForcedStrum = ExplicitHopo,
	/// <summary>
	/// The chord can be played without strumming
	/// </summary>
	Tap = 1 << 2,
	Big = 1 << 3
}

/// <summary>
/// Standard five-fret instruments
/// </summary>
/// <remarks>Can be cast to <see cref="InstrumentIdentity"/>.</remarks>
public enum StandardInstrumentIdentity : byte
{
	/// <inheritdoc cref="InstrumentIdentity.StandardLeadGuitar"/>
	LeadGuitar = InstrumentIdentity.StandardLeadGuitar,
	/// <inheritdoc cref="InstrumentIdentity.StandardRhythmGuitar"/>
	RhythmGuitar = InstrumentIdentity.StandardRhythmGuitar,
	/// <inheritdoc cref="InstrumentIdentity.StandardCoopGuitar"/>
	CoopGuitar = InstrumentIdentity.StandardCoopGuitar,
	/// <inheritdoc cref="InstrumentIdentity.StandardBass"/>
	Bass = InstrumentIdentity.StandardBass,
	/// <inheritdoc cref="InstrumentIdentity.StandardKeys"/>
	Keys = InstrumentIdentity.StandardKeys
}

/// <summary>
/// Lanes for a standard note
/// </summary>
public enum StandardLane : byte
{
	/// <summary>
	/// Open note with no associated fret
	/// </summary>
	/// <remarks>
	///     <para>Played by strumming with no frets pressed.</para>
	///     <para>When modified with <see cref="StandardChordModifiers.Tap"/>, is played by not having frets pressed at the time of the note.</para>
	/// </remarks>
	Open,
	/// <summary>
	/// First lane from the left
	/// </summary>
	/// <remarks>Actual position and color can very with gameplay modifiers.</remarks>
	Green,
	/// <summary>
	/// Second lane from the left
	/// </summary>
	/// <remarks>Actual position and color can very with gameplay modifiers.</remarks>
	Red,
	/// <summary>
	/// Third lane from the left
	/// </summary>
	/// <remarks>Actual position and color can very with gameplay modifiers.</remarks>
	Yellow,
	/// <summary>
	/// Fourth lane from the left
	/// </summary>
	/// <remarks>
	///     <para>Actual position and color can very with gameplay modifiers.</para>
	///     <para>Should only appear on tracks <see cref="Difficulty.Medium"/> and up.</para>
	/// </remarks>
	Blue,
	/// <summary>
	/// Fifth lane from the left
	/// </summary>
	/// <remarks>
	///     <para>Actual position and color can very with gameplay modifiers.</para>
	///     <para>Should only appear on tracks <see cref="Difficulty.Hard"/> and up.</para>
	/// </remarks>
	Orange
}

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
