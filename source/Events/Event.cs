using System;

namespace ChartTools
{
    /// <summary>
    /// Marker that defines an occurrence at a given point in a song.
    /// </summary>
    public abstract class Event : TrackObject
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
            set => EventData = string.IsNullOrEmpty(value) ? EventTypeString : EventTypeString + ' ' + value;
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
    }
}
