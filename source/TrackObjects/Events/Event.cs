using System;

namespace ChartTools
{
    /// <summary>
    /// Marker that defines an occurrence at a given point in a song.
    /// </summary>
    public abstract class Event : TrackObject, IEquatable<Event>
    {
        /// <summary>
        /// Type of event
        /// </summary>
        public string EventTypeString
        {
            get => EventData.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[0];
            set
            {
                string[] split = EventData.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                split[0] = value.Trim();

                EventData = string.Join(' ', split);
            }
        }

        internal string EventData { get; set; } = "Default";

        /// <summary>
        /// Creates an instance of <see cref="Event"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="eventData">Value of <see cref="EventData"/></param>
        internal Event(uint position, string eventData) : base(position) => EventData = eventData is null ?
            throw new CommonExceptions.ParameterNullException(nameof(eventData), 1)
            : eventData;

        public bool Equals(Event other) => base.Equals(other) && EventData == other.EventData;
    }
}