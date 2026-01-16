namespace ChartTools;

/// <summary>
/// Provides a proxy for accessing and modifying a note within a specific lane in a lane note collection.
/// </summary>
/// <remarks>Use <see cref="NoteProxy{TNote, TLane}"/> to interact with a note in a specific lane without directly
/// accessing the underlying collection. This can simplify code that needs to read or update notes for a particular
/// lane.</remarks>
/// <typeparam name="TNote">The value type representing a note associated with a lane. Must implement <see cref="ILaneNote{TLane}"/>.</typeparam>
/// <typeparam name="TLane">The enumeration type that identifies lanes within the collection.</typeparam>
public struct NoteProxy<TNote, TLane>
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : Enum
{
	public NoteProxy(TLane lane, LaneNoteCollection<TNote, TLane> source)
	{
		Source = source;
		Lane  = lane;
	}

	private int m_index = -1;

	public LaneNoteCollection<TNote, TLane> Source { get; }
	public TLane Lane { get; }

	public TNote? Get()
		=> Source[Lane];

	public ref readonly TNote GetUnsafe()
	{
		ReadOnlySpan<TNote> span = Source.AsSpan();

		if (m_index == -1)
		{
			for (int i = 0; i < span.Length; i++)
				if (span[i].Lane.Equals(Lane))
				{
					m_index = i;
					return ref span[i];
				}

			throw new KeyNotFoundException();
		}

		return ref span[m_index];
	}

	public void Set(in TNote note)
		=> Source.Add(in note);

	public static implicit operator TNote?(NoteProxy<TNote, TLane> proxy)
		=> proxy.Get();
}
