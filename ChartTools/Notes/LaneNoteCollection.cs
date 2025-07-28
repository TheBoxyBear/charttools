using System.Collections;
using System.Runtime.InteropServices;

namespace ChartTools;

public class LaneNoteCollection<TNote, TLane>(bool openExclusivity) : ICollection<TNote>, IReadOnlyList<TNote>
	where TNote : struct, ILaneNote<TLane>
	where TLane : struct, Enum
{
	private readonly List<TNote> m_notes = [];

	/// <summary>
	/// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
	/// </summary>
	public bool OpenExclusivity { get; } = openExclusivity;

	public int Count => m_notes.Count;

	bool ICollection<TNote>.IsReadOnly => false;

	public Span<TNote> AsSpan() => CollectionsMarshal.AsSpan(m_notes);

	public void Add(TLane lane) => Add(new TNote() { Lane = lane });

	/// <summary>
	/// Adds a note to the <see cref="LaneNoteCollection{TNote, TLane}"/>.
	/// </summary>
	/// <remarks>Adding a note that already exists will overwrite the existing note.
	///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
	/// </remarks>
	/// <param name="note">Note to add</param>
	public void Add(in TNote note)
	{
		// Will it defensive copy considering Index has a readonly getter?
		if (OpenExclusivity && (note.Index == 0 || Count == 1 && this[0].Index == 0)) // An open note is present and needs to be removed
			Clear();

		m_notes.Add(note);
	}

	void ICollection<TNote>.Add(TNote note) => Add(in note);

	public void AddRange(params ReadOnlySpan<TNote> notes) => m_notes.AddRange(notes);

	public void AddRange(params ReadOnlySpan<TLane> notes)
	{
		m_notes.Capacity += notes.Length;

		foreach (ref readonly TLane lane in notes)
			m_notes.Add(new TNote { Lane = lane });
	}

	public void Clear() => m_notes.Clear();

	/// <summary>
	/// Determines if any note matches the lane of a given note.
	/// </summary>
	/// <exception cref="ArgumentNullException"/>
	public bool Contains(in TNote note) => Contains(note.Lane);

	bool ICollection<TNote>.Contains(TNote note) => Contains(in note);

	/// <summary>
	/// Determines if any note matches a given lane.
	/// </summary>
	public bool Contains(TLane lane) => m_notes.Any(note => note.Lane.Equals(lane));

	/// <summary>
	/// Determines if any note matches a given index.
	/// </summary>
	public bool Contains(byte index) => m_notes.Any(note => note.Index == index);

	public void CopyTo(TNote[] array, int arrayIndex) => m_notes.CopyTo(array, arrayIndex);

	/// <summary>
	/// Removes the note that matches the lane of a given note.
	/// </summary>
	/// <returns><see langword="true"/> if a matching note was found.</returns>
	public bool Remove(in TNote note) => Remove(note.Lane);

	bool ICollection<TNote>.Remove(TNote note) => Remove(in note);

	/// <summary>
	/// Removes the note that matches a given lane.
	/// </summary>
	/// <returns><see langword="true"/> if a matching note was found.</returns>
	public bool Remove(TLane lane) => Remove(n => n.Lane.Equals(lane));

	/// <summary>
	/// Removes the note that matches a given index.
	/// </summary>
	/// <returns><see langword="true"/> if a matching note was found.</returns>
	public bool Remove(byte index) => Remove(n => n.Index == index);

	private bool Remove(Predicate<TNote> match)
	{
		int index = m_notes.FindIndex(match);

		if (index is -1)
			return false;

		m_notes.RemoveAt(index);
		return true;
	}

	/// <summary>
	/// Gets the note matching a given lane.
	/// </summary>
	/// <param name="lane">Lane of the note</param>
	/// <returns>Note with the lane if present, otherwise <see langword="null"/>.</returns>
	public TNote? this[TLane lane] => m_notes.FirstOrDefault(n => n.Lane.Equals(lane)); // TODO Throw exception instead of returning null

	/// <summary>
	/// Gets the note at a given index based on order or addition.
	/// </summary>
	/// <param name="index">Index of the note in the collection, not to be confused with <see cref="INote.Index"/>.</param>
	/// <returns>Note at the index</returns>
	/// <exception cref="ArgumentOutOfRangeException"/>
	public TNote this[int index] => m_notes[index];

	public IEnumerator<TNote> GetEnumerator() => m_notes.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => m_notes.GetEnumerator();
}
