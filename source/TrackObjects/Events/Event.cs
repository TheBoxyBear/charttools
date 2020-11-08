namespace ChartTools
{
    /// <summary>
    /// Marker that defines an occurence at a given point in a song.
    /// </summary>
    public abstract class Event : TrackObject
    {
        /// <summary>
        /// Type of event
        /// </summary>
        public string EventTypeString { get; set; }
        /// <summary>
        /// Additional data to modifiy the outcome of the event
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// Creates an instance of <see cref="Event"/>.
        /// </summary>
        /// <param name="position">Value of <see cref="TrackObject.Position"/></param>
        /// <param name="type">Value of <see cref="EventTypeString"/></param>
        /// <param name="argument">Value of <see cref="Argument"/></param>
        public Event(uint position, string type, string argument = "") : base(position)
        {
            EventTypeString = type;
            Argument = argument;
        }
    }
}