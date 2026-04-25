using ChartTools.Extensions;
using ChartTools.Extensions.Enums;

using System.Collections;
using System.Runtime.InteropServices;

namespace ChartTools;

public class LaneNoteCollection<TNote, TLane> : ILaneNoteCollection,
	ICollection<TNote>,
	IReadOnlyList<TNote>
	where TNote : struct, IDefinedLaneNote<TLane>
	where TLane : struct, Enum
{
	public bool OpenExclusivity { get; }
#if NET7_0_OR_GREATER
		= TNote.OpenExclusivity;
#else
		= default(TNote).OpenExclusivity;
#endif

	private readonly List<TNote> m_notes = [];

	public int Count
		=> m_notes.Count;

	bool ICollection<TNote>.IsReadOnly
		=> false;

	public ReadOnlySpan<TNote> AsSpan()
		=> CollectionsMarshal.AsSpan(m_notes);

	public void Add(SafeEnum<TLane> lane, uint sustain = 0)
		=> Add(new TNote
		{
			Lane = lane,
			Sustain = sustain
		});

	public void Add(byte laneIndex, uint sustain = 0)
		=> Add(UnsafeExtensions.AsReadonly<byte, TLane>(laneIndex), sustain);

	/// <summary>
	/// Adds a note to the <see cref="LaneNoteCollection{TNote, TLane}"/>.
	/// </summary>
	/// <remarks>Adding a note that already exists will overwrite the existing note.
	///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
	/// </remarks>
	/// <param name="note">Note to add</param>
	public void Add(in TNote note)
	{
		note.Lane.Validate();

		if (OpenExclusivity && (note.Index == 0 || Count == 1 && AsSpan()[0].Index == 0)) // An open note is present and needs to be removed
			Clear();

		Span<TNote> span = CollectionsMarshal.AsSpan(m_notes);

		// Try to find and replace existing note
		for (int i = 0; i < m_notes.Count; i++)
		{
			ref TNote thisNote = ref span[i];

			if (thisNote.Lane == note.Lane)
			{
				thisNote = note;
				return;
			}
		}

		m_notes.Add(note);

#if NET7_0_OR_GREATER
		if (m_notes.Capacity > TNote.MaxLanes)
			m_notes.Capacity = TNote.MaxLanes;
#else
		TNote dummy = default;

		if (m_notes.Capacity > dummy.MaxLanes)
			m_notes.Capacity = dummy.MaxLanes;
#endif
	}

	void ICollection<TNote>.Add(TNote note)
		=> Add(note);

	public void AddRange(params ReadOnlySpan<TNote> notes)
	{
		m_notes.Capacity += notes.Length;

		foreach (ref readonly TNote note in notes)
			Add(in note);
	}

	public void AddRange(params ReadOnlySpan<SafeEnum<TLane>> notes)
	{
		m_notes.Capacity += notes.Length;

		foreach (ref readonly SafeEnum<TLane> lane in notes)
			Add(new TNote { Lane = lane });
	}

	/// <summary>
	/// Removes all notes from the <see cref="LaneNoteCollection{TNote, TLane}"/>.
	/// </summary>
	public void Clear()
		=> m_notes.Clear();

	/// <summary>
	/// Determines if any note matches the lane of a given note.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public bool Contains(in TNote note)
	{
		foreach (ref readonly TNote thisNote in AsSpan())
			if (thisNote.Lane == note.Lane)
				return true;

		return false;
	}

	bool ICollection<TNote>.Contains(TNote note)
		=> Contains(in note);

	/// <summary>
	/// Determines if any note matches a given lane.
	/// </summary>
	public bool Contains(SafeEnum<TLane> lane)
		=> m_notes.Any(note => note.Lane == lane);

	bool ILaneNoteCollection.Contains(byte laneIndex)
		=> m_notes.Any(note => note.Index == laneIndex);

	public void CopyTo(TNote[] array, int arrayIndex)
		=> m_notes.CopyTo(array, arrayIndex);

	/// <summary>
	/// Removes the note that matches the lane of a given note.
	/// </summary>
	/// <returns><see langword="true"/> if a matching note was found.</returns>
	public bool Remove(in TNote note)
		=> Remove(note.Lane);

	bool ICollection<TNote>.Remove(TNote note)
		=> Remove(note.Lane);

	/// <summary>
	/// Removes the note that matches a given lane.
	/// </summary>
	/// <returns><see langword="true"/> if a matching note was found.</returns>
	public bool Remove(SafeEnum<TLane> lane)
		=> Remove((in note) => note.Lane == lane);

	bool ILaneNoteCollection.Remove(byte laneIndex)
		=> Remove((in note) => note.Index == laneIndex);

	private delegate bool Match(in TNote note);

	private bool Remove(Match match)
	{
		int removeIndex = -1;

		{
			ReadOnlySpan<TNote> span = AsSpan();

			for (int i = 0; i < span.Length; i++)
				if (match(in span[i]))
					removeIndex = i;
		}

		if (removeIndex is -1)
			return false;

		m_notes.RemoveAt(removeIndex);
		return true;
	}

	public NoteProxy<TNote, TLane>? Proxy(SafeEnum<TLane> lane)
	{
		TNote? note = this[lane];
		return note is null ? null : new NoteProxy<TNote, TLane>(lane, this);
	}

	NoteProxy? ILaneNoteCollection.Proxy(byte laneIndex)
		=> Proxy(UnsafeExtensions.AsReadonly<byte, TLane>(laneIndex));

	public IEnumerable<NoteProxy<TNote, TLane>> ProxyAll()
	{
		ReadOnlySpan<TNote> span = AsSpan();
		NoteProxy<TNote, TLane>[] proxies = new NoteProxy<TNote, TLane>[Count];

		for (int i = 0; i < Count; i++)
			proxies[i] = new NoteProxy<TNote, TLane>(span[i].Lane, this);

		return proxies;
	}

	IEnumerable<NoteProxy> ILaneNoteCollection.ProxyAll()
		=> ProxyAll().Select(static proxy => (NoteProxy)proxy);

	/// <summary>
	/// Gets the note matching a given lane.
	/// </summary>
	/// <param name="lane">Lane of the note</param>
	/// <returns>Note with the lane if present, otherwise <see langword="null"/>.</returns>
	public TNote? this[SafeEnum<TLane> lane]
	{
		get
		{
			foreach (ref readonly TNote note in AsSpan())
				if (note.Lane == lane)
					return note;

			return null;
		}
	}

	ILaneNote? ILaneNoteCollection.this[byte laneIndex]
		=> m_notes.FirstOrDefault(n => n.Lane.As<TLane, byte>() == laneIndex);

	TNote IReadOnlyList<TNote>.this[int index]
		=> m_notes[index];

	public IEnumerator<TNote> GetEnumerator()
		=> m_notes.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> m_notes.GetEnumerator();

	IEnumerator<ILaneNote> ILaneNoteCollection.GetEnumerator()
		=> m_notes.Cast<ILaneNote>().GetEnumerator();

	IEnumerable<ILaneNote> ILaneNoteCollection.AsEnumerable()
		=> m_notes.Cast<ILaneNote>();
}
