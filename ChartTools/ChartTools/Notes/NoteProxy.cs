using ChartTools.Extensions.Enums;

using System.Runtime.CompilerServices;

namespace ChartTools;

public readonly struct NoteProxy
{
	public readonly byte LaneIndex { get; }

	public readonly ILaneNoteCollection Source { get; }

	public NoteProxy(byte laneIndex, ILaneNoteCollection source)
	{
		ArgumentNullException.ThrowIfNull(source);

		LaneIndex = laneIndex;
		Source = source;
	}

	public readonly ILaneNote? Get()
		=> Source[LaneIndex];

	public void AddOrSet(uint sustain = 0)
		=> Source.Add(LaneIndex, sustain);
}

/// <summary>
/// Provides a proxy for accessing and modifying a note within a specific lane in a lane note collection.
/// </summary>
/// <remarks>Use <see cref="NoteProxy{TNote, TLane}"/> to interact with a note in a specific lane without directly
/// accessing the underlying collection. This can simplify code that needs to read or update notes for a particular
/// lane.</remarks>
/// <typeparam name="TNote">The value type representing a note associated with a lane. Must implement <see cref="ILaneNote{TLane}"/>.</typeparam>
/// <typeparam name="TLane">The enumeration type that identifies lanes within the collection.</typeparam>
public readonly struct NoteProxy<TNote, TLane>
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : struct, Enum
{
	private readonly int m_index = -1;

	public readonly SafeEnum<TLane> Lane { get; }

	public readonly byte LaneIndex => Lane.Value.As<TLane, byte>();

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

	public void AddOrSet(in TNote note)
	{
		if (note.Lane != Lane)
			throw new InvalidOperationException("The lane of the note does not match the proxy's lane.");

		Source.Add(in note);
	}

	public void AddOrSet(uint sustain = 0)
		=> Source.Add(Lane, sustain);

	public static implicit operator TNote?(in NoteProxy<TNote, TLane> proxy)
		=> proxy.Get();

	public static implicit operator NoteProxy(in NoteProxy<TNote, TLane> proxy)
		=> new(proxy.LaneIndex, proxy.Source);
}
