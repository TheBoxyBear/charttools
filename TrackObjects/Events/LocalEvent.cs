using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Event specific to an instrument and difficulty
    /// </summary>
    public class LocalEvent : Event
    {
        /// <summary>
        /// <see cref="Event.EventTypeString"/> value for each <see cref="LocalEventType"/>
        /// </summary>
        private static Dictionary<LocalEventType, string> localTypesDictionary = new Dictionary<LocalEventType, string>()
        {
            { LocalEventType.Solo, "solo" },
            { LocalEventType.SoloEnd, "soloend" },
            { LocalEventType.GHL6, "ghl_6" },
            { LocalEventType.GHL6Forced, "ghl_6_forced" },
            { LocalEventType.SoloOn, "solo_on" },
            { LocalEventType.SoloOff, "solo_off" },
            { LocalEventType.WailOn, "wail_on" },
            { LocalEventType.WailOff, "wail_off" },
            { LocalEventType.OwFaceOn, "ow_face_on" },
            { LocalEventType.OwFaceOff, "ow_face_off" }
        };

        /// <inheritdoc cref="Event.EventTypeString"/>
        public LocalEventType EventType
        {
            get => localTypesDictionary.ContainsValue(EventTypeString) ? localTypesDictionary.First(pair => pair.Value == EventTypeString).Key : LocalEventType.Unknown;
            set => EventTypeString = GetEventTypeString(value);
        }

        /// <summary>
        /// Creates an instance of <see cref="LocalEvent"/>.
        /// </summary>
        public LocalEvent(uint position, LocalEventType type, string argument = "") : base(position, GetEventTypeString(type), argument) { }
        /// <summary>
        /// Creates an instance of <see cref="LocalEvent"/>.
        /// </summary>
        internal LocalEvent(uint position, string type, string argument = "") : base(position, type, argument) { }

        /// <inheritdoc cref="GlobalEvent.GetEventTypeString(GlobalEventType)"/>
        private static string GetEventTypeString(LocalEventType type) => type == LocalEventType.Unknown ? "Default" : localTypesDictionary[type];
    }
}