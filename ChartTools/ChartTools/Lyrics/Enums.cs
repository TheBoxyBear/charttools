using ChartTools.Extensions.Enums;

namespace ChartTools.Lyrics;

/// <summary>
/// Keys making up <see cref="VocalsPitch"/> without the octave
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
	/// D-flat key
	/// </summary>
	Db = CSharp,
	/// <summary>
	/// D key (Ré)
	/// </summary>
	D,
	/// <summary>
	/// D# key
	/// </summary>
	DSharp,
	/// <summary>
	/// E-flat key
	/// </summary>
	Eb = DSharp,
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
	/// G-flat key
	/// </summary>
	Gb = FSharp,
	/// <summary>
	/// G key (Sol)
	/// </summary>
	G,
	/// <summary>
	/// G# key
	/// </summary>
	GSharp,
	/// <summary>
	/// A-flat key
	/// </summary>
	Ab = GSharp,
	/// <summary>
	/// A key (La)
	/// </summary>
	A,
	/// <summary>
	/// A# key
	/// </summary>
	ASharp,
	/// <summary>
	/// B-flat key
	/// </summary>
	Bb = ASharp,
	/// <summary>
	/// B key (Si)
	/// </summary>
	B
}

public enum VocalsPitch : byte
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
	/// Second D-flat
	/// </summary>
	Db2 = 0x20 | VocalsKey.Db,
	/// <summary>
	/// Second D
	/// </summary>
	D2 = 0x20 | VocalsKey.D,
	/// <summary>
	/// Second D#
	/// </summary>
	DSharp2 = 0x20 | VocalsKey.DSharp,
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
	/// Second G-flat
	/// </summary>
	Gb2 = 0x20 | VocalsKey.Gb,
	/// <summary>
	/// Second G
	/// </summary>
	G2 = 0x20 | VocalsKey.G,
	/// <summary>
	/// Second G#
	/// </summary>
	GSharp2 = 0x20 | VocalsKey.GSharp,
	/// <summary>
	/// Second A-flat
	/// </summary>
	Ab2 = 0x20 | VocalsKey.Ab,
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
	/// Third D-flat
	/// </summary>
	Db3 = 0x30 | VocalsKey.Db,
	/// <summary>
	/// Third D
	/// </summary>
	D3 = 0x30 | VocalsKey.D,
	/// <summary>
	/// Third D#
	/// </summary>
	DSharp3 = 0x30 | VocalsKey.DSharp,
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
	/// Third G-flat
	/// </summary>
	Gb3 = 0x30 | VocalsKey.Gb,
	/// <summary>
	/// Third G
	/// </summary>
	G3 = 0x30 | VocalsKey.G,
	/// <summary>
	/// Third G#
	/// </summary>
	GSharp3 = 0x30 | VocalsKey.GSharp,
	/// <summary>
	/// Third A-flat
	/// </summary>
	Ab3 = 0x30 | VocalsKey.Ab,
	/// <summary>
	/// Third A
	/// </summary>
	A3 = 0x30 | VocalsKey.A,
	/// <summary>
	/// Third A#
	/// </summary>
	ASharp3 = 0x30 | VocalsKey.ASharp,
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
	/// Fourth D-flat
	/// </summary>
	Db4 = 0x40 | VocalsKey.Db,
	/// <summary>
	/// Fourth D
	/// </summary>
	D4 = 0x40 | VocalsKey.D,
	/// <summary>
	/// Fourth D#
	/// </summary>
	DSharp4 = 0x40 | VocalsKey.DSharp,
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
	/// Fourth G-flat
	/// </summary>
	Gb4 = 0x40 | VocalsKey.Gb,
	/// <summary>
	/// Fourth G
	/// </summary>
	G4 = 0x40 | VocalsKey.G,
	/// <summary>
	/// Fourth G#
	/// </summary>
	GSharp4 = 0x40 | VocalsKey.GSharp,
	/// <summary>
	/// Fourth A-flat
	/// </summary>
	Ab4 = 0x40 | VocalsKey.Ab,
	/// <summary>
	/// Fourth A
	/// </summary>
	A4 = 0x40 | VocalsKey.A,
	/// <summary>
	/// Fourth A#
	/// </summary>
	ASharp4 = 0x40 | VocalsKey.ASharp,
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
	/// Fifth D-flat
	/// </summary>
	Db5 = 0x50 | VocalsKey.Db,
	/// <summary>
	/// Fifth D
	/// </summary>
	D5 = 0x50 | VocalsKey.D,
	/// <summary>
	/// Fifth D#
	/// </summary>
	DSharp5 = 0x50 | VocalsKey.DSharp,
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
	/// Fifth G-flat
	/// </summary>
	Gb5 = 0x50 | VocalsKey.Gb,
	/// <summary>
	/// Fifth G
	/// </summary>
	G5 = 0x50 | VocalsKey.G,
	/// <summary>
	/// Fifth G#
	/// </summary>
	GSharp5 = 0x50 | VocalsKey.GSharp,
	/// <summary>
	/// Fifth A-flat
	/// </summary>
	Ab5 = 0x50 | VocalsKey.Ab,
	/// <summary>
	/// Fifth A
	/// </summary>
	A5 = 0x50 | VocalsKey.A,
	/// <summary>
	/// Fifth A#
	/// </summary>
	ASharp5 = 0x50 | VocalsKey.ASharp,
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

public static class VocalsPitchExtensions
{
	extension(VocalsPitch pitch)
	{
		/// <summary>
		/// Key excluding the octave
		/// </summary>
		public VocalsKey Key
			=> (VocalsKey)((int)pitch & 0x0F);

		/// <summary>
		/// Octave number
		/// </summary>
		public byte Octave
			=> (byte)(((int)pitch & 0xF0) >> 4);
	}

	extension(SafeEnum<VocalsPitch> pitch)
	{
		/// <summary>
		/// Key excluding the octave
		/// </summary>
		public VocalsKey Key
			=> pitch.Value.Key;

		/// <summary>
		/// Octave number
		/// </summary>
		public byte Octave
			=> pitch.Value.Octave;
	}
}
