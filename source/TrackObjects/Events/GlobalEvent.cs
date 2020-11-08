using ChartTools.IO;
using ChartTools.IO.Chart;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Event common to all instruments
    /// </summary>
    public class GlobalEvent : Event
    {
        /// <summary>
        /// <see cref="Event.EventTypeString"/> value for each <see cref="GlobalEventType"/>
        /// </summary>
        private static readonly Dictionary<GlobalEventType, string> globalTypesDictionary = new Dictionary<GlobalEventType, string>()
        {
            { GlobalEventType.PhraseStart, "phrase_start" },
            { GlobalEventType.PhraseEnd, "phrase_end" },
            { GlobalEventType.Lyric, "lyric" },
            { GlobalEventType.Idle, "idle" },
            { GlobalEventType.Play, "play" },
            { GlobalEventType.HalfTempo, "half_tempo" },
            { GlobalEventType.NormalTempo, "normal_tempo" },
            { GlobalEventType.Verse, "verse" },
            { GlobalEventType.Chorus, "chorus" },
            { GlobalEventType.End, "end" },
            { GlobalEventType.MusicStart, "music_start" },
            { GlobalEventType.Lighting, "lighting ()" },
            { GlobalEventType.LightingFlare, "lighting (flare)" },
            { GlobalEventType.LightingBlackout, "lighting (blackout)" },
            { GlobalEventType.LightingChase, "lighting (chase)" },
            { GlobalEventType.LightingStrobe, "lighting (strobe)" },
            { GlobalEventType.LightingColor1, "lighting (color1)" },
            { GlobalEventType.LightingColor2, "lighting (color2)" },
            { GlobalEventType.LightingSweep, "lighting (sweep)" },
            { GlobalEventType.CrowdLightersFast, "crowd_lighters_fast" },
            { GlobalEventType.CrowdLightersOff, "crowd_lighters_off" },
            { GlobalEventType.CrowdLightersSlow, "crowd_lighters_slow" },
            { GlobalEventType.CrowdHalfTempo, "crowd_half_tempo" },
            { GlobalEventType.CrowdNormalTempo, "crowd_normal_tempo" },
            { GlobalEventType.CrowdDoubleTempo, "crowd_double_tempo" },
            { GlobalEventType.BandJump, "band_jump" },
            { GlobalEventType.Section, "section" },
            { GlobalEventType.SyncHeadBang, "sync_head_bang" },
            { GlobalEventType.SyncWag, "sync_wag" }
        };

        /// <inheritdoc cref="Event.EventTypeString"/>
        public GlobalEventType EventType
        {
            get => globalTypesDictionary.ContainsValue(EventTypeString) ? globalTypesDictionary.First(pair => pair.Value == EventTypeString).Key : GlobalEventType.Unknown;
            set => GetEventTypeString(value);
        }

        /// <summary>
        /// Determines if the <see cref="GlobalEvent"/> serves to display lyrics
        /// </summary>
        public bool IsLyricEvent
        {
            get
            {
                GlobalEventType type = EventType;
                return type == GlobalEventType.PhraseStart || type == GlobalEventType.Lyric || type == GlobalEventType.PhraseEnd;
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="GlobalEvent"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventType"/></param>
        /// <param name="argument">Value of <see cref="Event.Argument"/></param>
        public GlobalEvent(uint position, GlobalEventType type, string argument = "") : base(position, GetEventTypeString(type), argument) { }
        /// <summary>
        /// Creates an instance of <see cref="GlobalEvent"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventTypeString"/></param>
        /// <param name="argument">Value of <see cref="Argument"/></param>
        internal GlobalEvent(uint position, string type, string argument = "") : base(position, type, argument) { }

        /// <summary>
        /// Gets the string value to set <see cref="Event.EventTypeString"/>.
        /// </summary>
        /// <param name="type">Event type to get the string value of</param>
        private static string GetEventTypeString(GlobalEventType type) => type == GlobalEventType.Unknown ? "Default" : globalTypesDictionary[type];

        /// <inheritdoc cref="ChartParser.ReadGlobalEvents(string)"/>
        public static IEnumerable<GlobalEvent> FromFile(string path) => ExtensionHandler.Read(path, (".chart", ChartParser.ReadGlobalEvents));
    }
}