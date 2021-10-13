using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.SystemExtensions.Linq;

using System;
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
        /// Format in which events will be written when set to the <see cref="GlobalEventType.Section"/> type
        /// </summary>
        public static RockBandSectionFormat GlobalRockBandSectionFormat { get; set; }

        /// <summary>
        /// <see cref="Event.EventTypeString"/> value for each <see cref="GlobalEventType"/>
        /// </summary>
        private static readonly Dictionary<GlobalEventType, string> globalTypesDictionary = new()
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
            { GlobalEventType.Lighting, "lighting" },
            { GlobalEventType.CrowdLightersFast, "crowd_lighters_fast" },
            { GlobalEventType.CrowdLightersOff, "crowd_lighters_off" },
            { GlobalEventType.CrowdLightersSlow, "crowd_lighters_slow" },
            { GlobalEventType.CrowdHalfTempo, "crowd_half_tempo" },
            { GlobalEventType.CrowdNormalTempo, "crowd_normal_tempo" },
            { GlobalEventType.CrowdDoubleTempo, "crowd_double_tempo" },
            { GlobalEventType.BandJump, "band_jump" },
            //{ GlobalEventType.Section, "section" },
            //{ GlobalEventType.Section, "prc_" },
            { GlobalEventType.SyncHeadBang, "sync_head_bang" },
            { GlobalEventType.SyncWag, "sync_wag" }
        };
        private static readonly Dictionary<LightingEffect, string> lightingTypeDictionary = new()
        {
            { ChartTools.LightingEffect.Unknwon, "" },
            { ChartTools.LightingEffect.Flare, "flare" },
            { ChartTools.LightingEffect.Blackout, "blackout" },
            { ChartTools.LightingEffect.Chase, "chase" },
            { ChartTools.LightingEffect.Strobe, "strobe" },
            { ChartTools.LightingEffect.Color1, "color1" },
            { ChartTools.LightingEffect.Color2, "color2" },
            { ChartTools.LightingEffect.Sweep, "sweep" }
        };

        /// <inheritdoc cref="Event.EventTypeString"/>
        public GlobalEventType EventType
        {
            get
            {
                if (EventTypeString is "section" or "prc_")
                    return GlobalEventType.Section;

                return globalTypesDictionary.TryGetFirst(p => p.Value == EventTypeString, out KeyValuePair<GlobalEventType, string> pair)
                    ? pair.Key
                    : GlobalEventType.Unknown;
            }
            set => EventTypeString = GetEventTypeString(value);
        }

        /// <summary>
        /// The event serves to display lyrics
        /// </summary>
        public bool IsLyricEvent
        {
            get
            {
                GlobalEventType type = EventType;
                return type is GlobalEventType.PhraseStart or GlobalEventType.Lyric or GlobalEventType.PhraseEnd;
            }
        }
        /// <summary>
        /// The event controls the crowd
        /// </summary>
        public bool IsCrowdEvent => EventTypeString.StartsWith("crowd_");

        /// <summary>
        /// Additional data to modify the outcome of the event
        /// </summary>
        /// <remarks>A lack of argument is represented as an empty string.</remarks>
        public string Argument
        {
            get
            {
                string[] split = EventData.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                return split.Length > 1 ? split[1] : string.Empty;
            }
            set
            {
                if (string.IsNullOrEmpty(EventData))
                {
                    EventData = $"Default {value}";
                    return;
                }

                string[] split = EventData.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                split[1] = value;

                EventData = string.Join(' ', split);
            }
        }

        /// <summary>
        /// Rock Band format the section event is written in
        /// </summary>
        /// <remarks><see langword="null"/> if the event is not a section.</remarks>
        public RockBandSectionFormat? RockBandSectionFormat
        {
            get => EventTypeString switch
            {
                "section" => ChartTools.RockBandSectionFormat.RockBand2,
                "prc_" => ChartTools.RockBandSectionFormat.RockBand3,
                _ => null
            };
            set
            {
                if (value is not null)
                    EventTypeString = value switch
                    {
                        ChartTools.RockBandSectionFormat.RockBand2 => "section",
                        ChartTools.RockBandSectionFormat.RockBand3 => "prc_",
                        _ => throw CommonExceptions.GetUndefinedException(value.Value)
                    };
            }
        }

        /// <summary>
        /// Effect caused by the event if it is a lighting event
        /// </summary>
        /// <remarks><see langword="null"/> if the event is not a lighting event</remarks>
        public LightingEffect? LightingEffect
        {
            get
            {
                if (EventType is not GlobalEventType.Lighting)
                    return null;

                string value = LightingEffectSting!;

                return lightingTypeDictionary.TryGetFirst(p => p.Value == value, out KeyValuePair<LightingEffect, string> pair) ? pair.Key : ChartTools.LightingEffect.Unknwon;
            }
            set
            {
                if (value is not null)
                {
                    if (EventType is not GlobalEventType.Lighting)
                        EventType = GlobalEventType.Lighting;

                    LightingEffectSting = lightingTypeDictionary.TryGetValue(value.Value, out string? arg) ? arg : string.Empty;
                }

            }
        }
        /// <summary>
        /// Lighting effect as it is written in the file
        /// </summary>
        public string? LightingEffectSting
        {
            get => EventType is not GlobalEventType.Lighting ? null : Argument.TrimStart('(').TrimEnd(')');
            set
            {
                if (EventType is not GlobalEventType.Lighting)
                    Argument = $"({value})";
            }
        }

        /// <summary>
        /// Creates an instance of <see cref="GlobalEvent"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventType"/></param>
        /// <param name="argument">Value of <see cref="Event.Argument"/></param>
        public GlobalEvent(uint position, GlobalEventType type, string argument = "") : this(position, GetEventTypeString(type), argument) { }
        /// <summary>
        /// Creates an instance of <see cref="GlobalEvent"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventTypeString"/></param>
        /// <param name="argument">Value of <see cref="Argument"/></param>
        public GlobalEvent(uint position, string type, string argument = "") : base(position, type + (string.IsNullOrEmpty(argument) ? string.Empty : $" {argument}")) { }
        /// <summary>
        /// Gets the string value to set <see cref="Event.EventTypeString"/>.
        /// </summary>
        /// <param name="type">Event type to get the string value of</param>
        internal static string GetEventTypeString(GlobalEventType type) => type switch
        {
            GlobalEventType.Unknown => "Default",
            GlobalEventType.Section => GlobalRockBandSectionFormat == ChartTools.RockBandSectionFormat.RockBand2 ? "section" : "prc_",
            _ => globalTypesDictionary[type]
        };

        /// <inheritdoc cref="ChartParser.ReplaceGlobalEvents(string, IEnumerable{GlobalEvent})"/>
        public static void ToFile(string path, IEnumerable<GlobalEvent> events, WritingConfiguration? config = default) => ExtensionHandler.Write(path, events, config, (".chart", ChartParser.ReplaceGlobalEvents));
    }
}
