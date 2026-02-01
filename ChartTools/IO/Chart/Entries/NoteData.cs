namespace ChartTools.IO.Chart.Entries;

/// <summary>
/// Line of chart data representing a <see cref="LaneNote{TLane}.Sustain"/>
/// </summary>
internal readonly ref struct NoteData
{
	/// <summary>
	/// Value of <see cref="LaneNote{TLane}.Sustain"/>
	/// </summary>
	internal readonly byte Index;

	/// <summary>
	/// Value of <see cref="LaneNote{TLane}.Sustain"/>
	/// </summary>
	internal readonly uint SustainLength;

	/// <summary>
	/// Creates an instance of <see cref="NoteData"/>.
	/// </summary>
	/// <param name="data">Data section of the line in the file</param>
	/// <exception cref="FormatException"/>
	internal NoteData(in ReadOnlySpan<char> data)
	{
		ReadOnlySpan<char> a, b;
		ChartFormatting.SplitData(data, out a, out b);

		if (b.IsEmpty)
			throw new EntryException();

		Index         = ValueParser.Parse<byte>(in a, "note index");
		SustainLength = ValueParser.Parse<uint>(in b, "sustain length");
	}
}
