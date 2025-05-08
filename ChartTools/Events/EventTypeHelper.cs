namespace ChartTools.Events;

/// <summary>
/// Provides string definitions for supported types of <see cref="GlobalEvent"/> and <see cref="LocalEvent"/>.
/// </summary>
public static class EventTypeHelper
{
	/// <summary>
	/// Types shared between <see cref="GlobalEvent"/> and <see cref="LocalEvent"/>
	/// </summary>
	public static class Common
	{
		public const string ToggleOn = "on";
		public const string ToggleOff = "off";
	}

	/// <summary>
	/// Types for <see cref="GlobalEvent"/>
	/// </summary>
	public static class Global
	{
		public const string
			BandJump = "band_jump",
			BassistIdle = EventTypeHeaderHelper.Global.BassistMovement + Common.ToggleOff,
			BassistMove = EventTypeHeaderHelper.Global.BassistMovement + Common.ToggleOn,
			DrummerIdle = EventTypeHeaderHelper.Global.DrummerMovement + Common.ToggleOff,
			DrummerAll = EventTypeHeaderHelper.Global.DrummerMovement + "allbeat",
			DrummerDouble = EventTypeHeaderHelper.Global.DrummerMovement + "double",
			DrummerHalf = EventTypeHeaderHelper.Global.DrummerMovement + "half",
			DrummerMove = EventTypeHeaderHelper.Global.DrummerMovement + Common.ToggleOn,
			Chorus = "chorus",
			CrowdLightersFast = EventTypeHeaderHelper.Global.Crowd + "lighters_fast",
			CrowdLightersOff = EventTypeHeaderHelper.Global.Crowd + "lighters_off",
			CrowdLightersSlow = EventTypeHeaderHelper.Global.Crowd + "lighters_slow",
			CrowdHalfTempo = EventTypeHeaderHelper.Global.Crowd + "half_tempo",
			CrowdNormalTempo = EventTypeHeaderHelper.Global.Crowd + "normal_tempo",
			CrowdDoubleTempo = EventTypeHeaderHelper.Global.Crowd + "double_tempo",
			End = "end",
			GuitaristIdle = EventTypeHeaderHelper.Global.GuitaristMovement + Common.ToggleOff,
			GuitaristMove = EventTypeHeaderHelper.Global.GuitaristMovement + Common.ToggleOn,
			GuitaristSoloOn = EventTypeHeaderHelper.Global.GuitaristSolo + Common.ToggleOn,
			GuitaristSoloOff = EventTypeHeaderHelper.Global.GuitaristSolo + Common.ToggleOff,
			GuitaristWailOn = EventTypeHeaderHelper.Global.GuitaristWail + Common.ToggleOn,
			GuitaristWailOff = EventTypeHeaderHelper.Global.GuitaristWail + Common.ToggleOff,
			HalfTempo = "half_tempo",
			Idle = "idle",
			KeyboardIdle = EventTypeHeaderHelper.Global.SingerMovement + Common.ToggleOff,
			KeyboardMove = EventTypeHeaderHelper.Global.SingerMovement + Common.ToggleOn,
			Lighting = "lighting",
			MusicStart = "music_start",
			NotrmalTempo = "normal_tempo",
			Play = "play",
			RB2CHSection = "section",
			RB3Section = "prc_",
			SingerIdle = EventTypeHeaderHelper.Global.SingerMovement + Common.ToggleOff,
			SingerMove= EventTypeHeaderHelper.Global.SingerMovement + Common.ToggleOn,
			SyncHeadBang = EventTypeHeaderHelper.Global.Sync + "head_bang",
			SyncWag = EventTypeHeaderHelper.Global.Sync + "wag",
			Verse = "verse";

		/// <summary>
		/// Individual lyric syllable with the syllable text as an event argument
		/// </summary>
		/// <remarks>
		///     <para>Must be placed after a <see cref="PhraseStart"/> event and before the matching <see cref="PhraseEnd"/> event if present.</para>
		///     <para>If on the same tick as the <see cref="PhraseStart"/> event, must appear after said event in the list.</para>
		/// </remarks>
		public const string Lyric = "lyric";

		/// <summary>
		/// Start of a lyrics phrase
		/// </summary>
		/// <remarks>Can be placed on the same tick as the next <see cref="Lyric"/> event, in which case it must appear before said event in the list.</remarks>
		public const string PhraseStart = EventTypeHeaderHelper.Global.Phrase + "start";

		/// <summary>
		/// End of a lyrics phrase
		/// </summary>
		/// <remarks>
		///     <para>Requires a prior <see cref="PhraseStart"/> event.</para>
		///     <para>Optional. If absent, the phrase lasts until the next <see cref="PhraseStart"/> event or the end of the song. Usage recommended for proper clearing of lyrics.</para>
		/// </remarks>
		public const string PhraseEnd = EventTypeHeaderHelper.Global.Phrase + "end";
	}

	/// <summary>
	/// Types for <see cref="LocalEvent"/>
	/// </summary>
	public static class Local
	{
		public const string
			GHL6       = EventTypeHeaderHelper.Local.GHL6,
			GHL6Forced = EventTypeHeaderHelper.Local.GHL6 + "_forced",
			OwFaceOn   = EventTypeHeaderHelper.Local.OwFace + Common.ToggleOn,
			OwFaceOff  = EventTypeHeaderHelper.Local.OwFace + Common.ToggleOff;

		/// <summary>
		/// Start of a solo section awarding bonus points
		/// </summary>
		/// <remarks>Section ended with <see cref="SoloEnd"/>.</remarks>
		public const string Solo = "solo";

		/// <summary>
		/// End of a solo section.
		/// </summary>
		/// <remarks>Requires a prior <see cref="Solo"/> event.</remarks>
		public const string SoloEnd = "soloend";
	}
}
