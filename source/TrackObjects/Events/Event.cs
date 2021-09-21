using System;

namespace ChartTools
{
    /// <summary>
    /// Marker that defines an occurrence at a given point in a song.
    /// </summary>
    public abstract class Event : TrackObject, IEquatable<Event>
    {
        /// <summary>
        /// Type of event as it is written in the file
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
        protected Event(uint position, string eventData) : base(position) => EventData = eventData is null
            ? throw new ArgumentNullException(nameof(eventData))
            : eventData;

        public override bool Equals(TrackObject? other) => Equals(other as Event);
        public bool Equals(Event? other) => base.Equals(other) && EventData == other.EventData;

        public override string ToString() => $"Event: {EventData}";
    }
}
