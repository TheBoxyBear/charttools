using System;

namespace ChartTools
{
    /// <summary>
    /// Marker that defines an occurence at a given point in a song.
    /// </summary>
    public abstract class Event : TrackObject, IEquatable<Event>
    {
        /// <summary>
        /// Type of event
        /// </summary>
        public string EventTypeString
        {
            get => EventData?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[0];
            set
            {
                if (EventData is null)
                {
                    EventData = value;
                    return;
                }

                string[] split = EventData.Split(' ', 2);
                split[0] = value;

                EventData = string.Join(' ', split);
            }
        }

        internal string EventData { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="Event"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="eventData">Value of <see cref="EventData"/></param>
        internal Event(uint position, string eventData) : base(position)
        {
            if (eventData is null)
                throw CommonExceptions.GetNullParameterException("eventData");
        }

        public bool Equals(Event other) => base.Equals(other) && EventData == other.EventData;
    }
}