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
        private static readonly Dictionary<LocalEventType, string> localTypesDictionary = new()
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

        public bool IsStarPowerEvent => EventType is LocalEventType.Solo or LocalEventType.SoloEnd;

        /// <summary>
        /// Creates an instance of <see cref="LocalEvent"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventType"/></param>
        /// <param name="argument">Value of <see cref="Event.Argument"/></param>
        public LocalEvent(uint position, LocalEventType type) : base(position, GetEventTypeString(type)) { }
        /// <summary>
        /// Creates an instance of <see cref="LocalEvent"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventTypeString"/></param>
        /// <param name="argument">Value of <see cref="Argument"/></param>
        public LocalEvent(uint position, string type) : base(position, type) { }

        /// <inheritdoc cref="GlobalEvent.GetEventTypeString(GlobalEventType)"/>
        private static string GetEventTypeString(LocalEventType type) => type == LocalEventType.Unknown ? "Default" : localTypesDictionary[type];
    }
}
