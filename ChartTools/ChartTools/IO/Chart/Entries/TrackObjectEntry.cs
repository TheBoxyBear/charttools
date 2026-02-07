namespace ChartTools.IO.Chart.Entries;

/// <summary>
/// Line of chart file data representing a <see cref="TrackObjectBase"/>
/// </summary>
internal readonly struct TrackObjectEntry : IReadOnlyTrackObject
{
	/// <summary>
	/// Value of <see cref="ITrackObject.Position"/>
	/// </summary>
	public uint Position { get; }

	/// <summary>
	/// Type code of <see cref="ITrackObject"/>
	/// </summary>
	public ReadOnlyMemory<char> Type { get; }

	/// <summary>
	/// Additional data
	/// </summary>
	public ReadOnlyMemory<char> Data { get; }

	public TrackObjectEntry(string line)
		: this(line.AsMemory()) { }

	/// <summary>
	/// Creates an instance of see<see cref="TrackObjectEntry"/>.
	/// </summary>
	/// <param name="line">Line in the file</param>
	/// <exception cref="LineException"/>
	public TrackObjectEntry(in ReadOnlyMemory<char> line)
	{
		TextEntry entry = new(line);

		if (entry.Value.IsEmpty)
			throw new LineException(line.ToString(), new FormatException("Line has no object data."));

		int spaceIndex = entry.Value.Span.IndexOf(' ');

		if (spaceIndex == -1)
			throw new LineException(line.ToString(), new EntryException());

		Type = entry.Value[0..spaceIndex];
		Data = entry.Value[(spaceIndex + 1)..];

		Position = ValueParser.Parse<uint>(entry.Key.Span, "position");
	}

	public TrackObjectEntry(uint position, string type, string data)
		: this(position, type.AsMemory(), data.AsMemory()) { }

	public TrackObjectEntry(uint position, in ReadOnlyMemory<char> type, in ReadOnlyMemory<char> data)
	{
		Position = position;
		Type     = type;
		Data     = data;
	}

	public override string ToString()
		=> ChartFormatting.Line(Position.ToString(), $"{Type} {Data}");
}
