namespace ChartTools.Events;

/// <summary>
/// Provides helper strings for composite event types
/// </summary>
public static class EventTypeHeaderHelper
{
    /// <summary>
    /// Helpers for <see cref="GlobalEvent"/>
    /// </summary>
	public static class Global
	{
		public const string
			BassistMovement   = "bass_",
			Crowd             = "crowd_",
			DrummerMovement   = "drum_",
			GuitaristMovement = "gtr_",
			GuitaristSolo     = "solo_",
			GuitaristWail     = "wail_",
			KeyboardMovement  = "keys_",
			Phrase            = "phrase_",
			SingerMovement    = "sing_",
			Sync              = "sync_";
	}

    /// <summary>
    /// Helpers for <see cref="LocalEvent"/>
    /// </summary>
	public static class Local
	{
		public const string
			GHL6   = "ghl_6",
			OwFace = "ow_face_";
	}
}
