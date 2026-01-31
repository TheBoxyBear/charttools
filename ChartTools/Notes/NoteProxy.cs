using System.Runtime.CompilerServices;

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
	where TLane : struct, Enum
{
	private readonly int m_index = -1;

	public readonly SafeEnum<TLane> Lane { get; }

	public readonly LaneNoteCollection<TNote, TLane> Source { get; }

	public NoteProxy(TLane lane, LaneNoteCollection<TNote, TLane> source)
	{
		ArgumentNullException.ThrowIfNull(source);

		Lane   = lane;
		Source = source;
	}

	public readonly TNote? Get()
		=> Source[Lane];

	public readonly ref readonly TNote GetUnsafe()
	{
		ReadOnlySpan<TNote> span = Source.AsSpan();

		if (m_index == -1)
		{
			for (int i = 0; i < span.Length; i++)
				if (span[i].Lane == Lane)
				{
					Unsafe.AsRef(in m_index) = i;
					return ref span[i];
				}

			throw new KeyNotFoundException();
		}

		return ref span[m_index];
	}

	public void Set(in TNote note)
	{
		if (!note.Lane.Equals(Lane))
			throw new InvalidOperationException("The lane of the note does not match the proxy's lane.");

		Source.Add(in note);
	}

	public static implicit operator TNote?(in NoteProxy<TNote, TLane> proxy)
		=> proxy.Get();
}
