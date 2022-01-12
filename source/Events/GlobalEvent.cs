﻿using ChartTools.IO;
using ChartTools.IO.Chart;
using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        public static SectionEventFormat GlobalRockBandSectionFormat { get; set; }

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
            get => EventTypeString is "section" or "prc_"
                    ? GlobalEventType.Section
                    : globalTypesDictionary.TryGetFirst(p => p.Value == EventTypeString, out KeyValuePair<GlobalEventType, string> pair)
                    ? pair.Key
                    : GlobalEventType.Unknown;

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
        /// Rock Band format the section event is written in
        /// </summary>
        /// <remarks><see langword="null"/> if the event is not a section.</remarks>
        public SectionEventFormat? SectionFormat
        {
            get => EventTypeString switch
            {
                "section" => SectionEventFormat.RockBand2CloneHero,
                "prc_" => SectionEventFormat.RockBand3,
                _ => null
            };
            set
            {
                if (value is not null)
                    EventTypeString = value switch
                    {
                        SectionEventFormat.RockBand2CloneHero => "section",
                        SectionEventFormat.RockBand3 => "prc_",
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
            _ => globalTypesDictionary[type]
        };

        /// <summary>
        /// Reads global events from a file.
        /// </summary>
        /// <param name="path">Path of the file</param>
        public static IEnumerable<GlobalEvent> FromFile(string path) => ExtensionHandler.Read<IEnumerable<GlobalEvent>>(path, null, (".chart", (path, _) => ChartReader.ReadGlobalEvents(path)));
        /// <summary>
        /// Reads global events from a file asynchronously using multitasking.
        /// </summary>
        /// <param name="path"><inheritdoc cref="FromFile(string)" path="/param[@name='path']"/></param>
        /// <param name="cancellationToken">Token to request cancellation</param>
        public static async Task<List<GlobalEvent>> FromFileAsync(string path, CancellationToken cancellationToken) => await ExtensionHandler.ReadAsync<List<GlobalEvent>>(path, cancellationToken, null, (".chart", (path, token, _) => ChartReader.ReadGlobalEventsAsync(path, token)));
    }
}
