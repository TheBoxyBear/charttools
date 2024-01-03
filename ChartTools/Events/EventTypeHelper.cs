namespace ChartTools.Events;

public static class EventTypeHelper
{
    public static class Common
    {
        public const string ToggleOn = "on";
        public const string ToggleOff = "off";
    }

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
            Lyric = "lyric",
            MusicStart = "music_start",
            NotrmalTempo = "normal_tempo",
            Play = "play",
            PhraseStart = EventTypeHeaderHelper.Global.Phrase + "start",
            PhraseEnd = EventTypeHeaderHelper.Global.Phrase + "end",
            RB2CHSection = "section",
            RB3Section = "prc_",
            SingerIdle = EventTypeHeaderHelper.Global.SingerMovement + Common.ToggleOff,
            SingerMove= EventTypeHeaderHelper.Global.SingerMovement + Common.ToggleOn,
            SyncHeadBang = EventTypeHeaderHelper.Global.Sync + "head_bang",
            SyncWag = EventTypeHeaderHelper.Global.Sync + "wag",
            Verse = "verse";
    }

    public static class Local
    {
        public const string
            GHL6 = EventTypeHeaderHelper.Local.GHL6,
            GHL6Forced = EventTypeHeaderHelper.Local.GHL6 + "_forced",
            OwFaceOn = EventTypeHeaderHelper.Local.OwFace + Common.ToggleOn,
            OwFaceOff = EventTypeHeaderHelper.Local.OwFace + Common.ToggleOff,
            Solo = "solo",
            SoloEnd = "soloend";
    }
}
