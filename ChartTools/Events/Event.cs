namespace ChartTools.Events;

/// <summary>
/// Marker that defines an occurrence at a given point in a song.
/// </summary>
public abstract class Event : ITrackObject
{
	public uint Position { get; set; }

	private string m_eventType = "Default";

	/// <summary>
	/// Type of event as it is written in the file
	/// </summary>
	public string EventType
	{
		get => m_eventType;
		set
		{
			if (string.IsNullOrEmpty(value))
				throw new FormatException("Event type is empty");

			if (value.Contains(' '))
				throw new FormatException("Event types cannot contain spaces");

			m_eventType = value;
		}
	}

	private string? m_argument = null;

	/// <summary>
	/// Additional data to modify the outcome of the event
	/// </summary>
	/// <remarks>A lack of argument is represented as an empty string.</remarks>
	public string? Argument
	{
		get => m_argument;
		set => m_argument = value ?? string.Empty;
	}

	/// <summary>
	/// Combined event type and arguments where the first word is the type.
	/// </summary>
	public string EventData
	{
		get => Argument is null ? EventType : string.Join(' ', EventType, Argument);
		set
		{
			string[] split = value.Split(' ', 2);

			EventType = split[0];
			Argument = split.Length > 1 ? split[1] : string.Empty;
		}
	}

	public bool? ToggleState => EventType.EndsWith(EventTypeHelper.Common.ToggleOn)
		? true : (EventType.EndsWith(EventTypeHelper.Common.ToggleOff) ? false : null);

	public Event(uint position, string data)
	{
		Position  = position;
		EventData = data;
	}

	public Event(uint position, string type, string? argument)
	{
		Position  = position;
		EventType = type;
		Argument  = argument;
	}

	public override string ToString() => EventData;
}
