namespace ChartTools.Events;

/// <summary>
/// Marker that defines an occurrence at a given point in a song.
/// </summary>
public abstract class Event(uint position) : ITrackObject
{
	public uint Position { get; set; } = position;

	/// <summary>
	/// Type of event as it is written in the file
	/// </summary>
	public string EventType
	{
		get;
		set
		{
			if (string.IsNullOrEmpty(value))
				throw new FormatException("Event type is empty");

			if (value.Contains(' '))
				throw new FormatException("Event types cannot contain spaces");

			field = value;
		}
	} = "Default";

	/// <summary>
	/// Additional data to modify the outcome of the event
	/// </summary>
	/// <remarks>A lack of argument is represented as an empty string.</remarks>
	public string Argument { get; set; } = string.Empty;

	/// <summary>
	/// Combined event type and arguments where the first word is the type.
	/// </summary>
	public string EventData
	{
		get => Argument == string.Empty ? EventType : string.Join(' ', EventType, Argument);
		set
		{
			// Can possibly be optimized with a stack array
			string[] split = value.Split(' ', 2);


			EventType = split[0];
			Argument = split.Length > 1 ? split[1] : string.Empty;
		}
	}

	public bool? ToggleState => EventType.EndsWith(EventTypeHelper.Common.ToggleOn)
		? true : (EventType.EndsWith(EventTypeHelper.Common.ToggleOff) ? false : null);

	public Event(uint position, string data)
		: this(position) => EventData = data;

	public Event(uint position, string type, string argument)
		: this(position)
	{
		EventType = type;
		Argument  = argument;
	}

	public override string ToString() => EventData;
}
