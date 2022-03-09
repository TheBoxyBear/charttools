namespace ChartTools.Events
{
    public static class EventTypeHelper
    {
        public static class Global
        {
            public const string
                PhraseStart = "phrase_start",
                PhraseEnd = "phrase_end",
                Lyric = "lyric",
                Idle = "idle",
                Play = "play",
                HalfTempo = "half_tempo",
                NotrmalTempo = "normal_tempo",
                Verse = "verse",
                Chorus = "chorus",
                End = "end",
                MusicStart = "music_start",
                Lighting = "lighting",
                CrowdLightersFast = EventTypeHeaderHelper.Global.Crowd + "lighters_fast",
                CrowdLightersOff = EventTypeHeaderHelper.Global.Crowd + "lighters_off",
                CrowdLightersSlow = EventTypeHeaderHelper.Global.Crowd + "lighters_slow",
                CrowdHalfTempo = EventTypeHeaderHelper.Global.Crowd + "half_tempo",
                CrowdNormalTempo = EventTypeHeaderHelper.Global.Crowd + "normal_tempo",
                CrowdDoubleTempo = EventTypeHeaderHelper.Global.Crowd + "double_tempo",
                BandJump = "band_jump",
                RB2CHSection = "section",
                RB3Section = "prc_",
                SyncHeadBang = EventTypeHeaderHelper.Global.Sync + "head_bang",
                SyncWag = EventTypeHeaderHelper.Global.Sync + "wag";
        }

        public static class Local
        {
            public const string
                Solo = EventTypeHeaderHelper.Local.Solo,
                SoloEnd = EventTypeHeaderHelper.Local.Solo + "end",
                SoloOn = EventTypeHeaderHelper.Local.Solo + "_on",
                SoloOff = EventTypeHeaderHelper.Local.Solo + "_off",
                GHL6 = EventTypeHeaderHelper.Local.GHL6,
                GHL6Forced = EventTypeHeaderHelper.Local.GHL6 + "_forced",
                WailOn = EventTypeHeaderHelper.Local.Wail + "on",
                WailOff = EventTypeHeaderHelper.Local.Wail + "off",
                OwFaceOn = EventTypeHeaderHelper.Local.OwFace + "on",
                OwFaceOff = EventTypeHeaderHelper.Local.OwFace + "off";
        }
    }
}
