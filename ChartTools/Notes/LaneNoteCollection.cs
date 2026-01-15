using System.Collections;
using System.Runtime.InteropServices;

namespace ChartTools;

public class LaneNoteCollection<TNote, TLane>(bool openExclusivity) : ILaneNoteCollection,
	ICollection<TNote>,
	IReadOnlyList<TNote>
	where TNote : struct, ILaneNote<TLane>
	where TLane : Enum
{
	public bool OpenExclusivity { get; } = openExclusivity;

	private readonly List<TNote> m_notes = [];

	public int Count
		=> m_notes.Count;

	bool ICollection<TNote>.IsReadOnly
		=> false;

	public ReadOnlySpan<TNote> AsSpan()
		=> CollectionsMarshal.AsSpan(m_notes);

	public void Add(TLane lane)
		=> Add(new TNote { Lane = lane });

	/// <summary>
	/// Adds a note to the <see cref="LaneNoteCollection{TNote, TLane}"/>.
	/// </summary>
	/// <remarks>Adding a note that already exists will overwrite the existing note.
	///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
	/// </remarks>
	/// <param name="note">Note to add</param>
	public void Add(in TNote note)
	{
		if (OpenExclusivity && (note.Index == 0 || Count == 1 && AsSpan()[0].Index == 0)) // An open note is present and needs to be removed
			Clear();

		m_notes.Add(note);
	}

	void ICollection<TNote>.Add(TNote note)
		=> Add(note);

	public void AddRange(params ReadOnlySpan<TNote> notes)
	{
		m_notes.Capacity += notes.Length;

		foreach (ref readonly TNote note in notes)
			Add(in note);
	}

	public void AddRange(params ReadOnlySpan<TLane> notes)
	{
		m_notes.Capacity += notes.Length;

		foreach (ref readonly TLane lane in notes)
			m_notes.Add(new TNote { Lane = lane });
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
			if (thisNote.Lane.Equals(note.Lane))
				return true;

		return false;
	}

	bool ICollection<TNote>.Contains(TNote note)
		=> Contains(in note);

	/// <summary>
	/// Determines if any note matches a given lane.
	/// </summary>
	public bool Contains(TLane lane)
		=> m_notes.Any(note => note.Lane.Equals(lane));

	bool ILaneNoteCollection.Contains(byte index)
		=> m_notes.Any(note => note.Index == index);

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
	public bool Remove(TLane lane)
		=> Remove((in note) => note.Lane.Equals(lane));

	bool ILaneNoteCollection.Remove(byte index)
		=> Remove((in note) => note.Index == index);

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

	public NoteProxy<TNote, TLane>? Proxy(TLane lane)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Gets the note matching a given lane.
	/// </summary>
	/// <param name="lane">Lane of the note</param>
	/// <returns>Note with the lane if present, otherwise <see langword="null"/>.</returns>
	public TNote? this[TLane lane]
		=> m_notes.FirstOrDefault(n => n.Lane.Equals(lane));

	ILaneNote? ILaneNoteCollection.this[byte index]
		=> m_notes.FirstOrDefault(n => n.Lane.Equals(index));

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
