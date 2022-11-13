using System.Collections;

namespace ChartTools;

public class LaneNoteCollection<TNote, TLane> : ICollection<TNote>, IReadOnlyList<TNote> where TNote : LaneNote<TLane>, new() where TLane : struct, Enum
{
    private readonly List<TNote> _notes = new();

    /// <summary>
    /// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
    /// </summary>
    public bool OpenExclusivity { get; }
    public int Count => _notes.Count;
    bool ICollection<TNote>.IsReadOnly => false;

    public LaneNoteCollection(bool openExclusivity) => OpenExclusivity = openExclusivity;

    public void Add(TLane lane) => AddNonNull(new TNote() { Lane = lane });
    /// <summary>
    /// Adds a note to the <see cref="LaneNoteCollection{TNote, TLane}"/>.
    /// </summary>
    /// <remarks>Adding a note that already exists will overwrite the existing note.
    ///     <para>If <see cref="OpenExclusivity"/> is <see langword="true"/>, combining an open note with other notes will remove the current ones.</para>
    /// </remarks>
    /// <param name="note">Note to add</param>
    public void Add(TNote note) => AddNonNull(note ?? throw new ArgumentNullException(nameof(note)));
    private void AddNonNull(TNote note)
    {
        if (OpenExclusivity && (note.Index == 0 || Count == 1 && this.First().Index == 0)) // An open note is present and needs to be removed
            Clear();
        float f = 1;
        int i = 1;

        var r = f / i;
        _notes.Add(note);
    }

    public void Clear() => _notes.Clear();

    /// <summary>
    /// Determines if any note matches the lane of a given note.
    /// </summary>
    /// <exception cref="ArgumentNullException"/>
    public bool Contains(TNote note) => note is null ? throw new ArgumentNullException(nameof(note)) : Contains(note.Lane);
    /// <summary>
    /// Determines if any note matches a given lane.
    /// </summary>
    public bool Contains(TLane lane) => _notes.Any(note => note.Lane.Equals(lane));
    /// <summary>
    /// Determines if any note matches a given index.
    /// </summary>
    public bool Contains(byte index) => _notes.Any(note => note.Index == index);

    public void CopyTo(TNote[] array, int arrayIndex) => _notes.CopyTo(array, arrayIndex);

    /// <summary>
    /// Removes the note that matches the lane of a given note.
    /// </summary>
    /// <returns><see langword="true"/> if a matching note was found.</returns>
    public bool Remove(TNote note) => Remove(note.Lane);
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
        var index = _notes.FindIndex(match);

        if (index is -1)
            return false;

        _notes.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Gets the note matching a given lane.
    /// </summary>
    /// <param name="lane">Lane of the note</param>
    /// <returns>Note with the lane if present, otherwise <see langword="null"/>.</returns>
    public TNote? this[TLane lane] => _notes.FirstOrDefault(n => n.Lane.Equals(lane));
    /// <summary>
    /// Gets the note at a given index based on order or addition.
    /// </summary>
    /// <param name="index">Index of the note in the collection, not to be confused with <see cref="INote.Index"/>.</param>
    /// <returns>Note at the index</returns>
    /// <exception cref="ArgumentOutOfRangeException"/>
    public TNote this[int index] => _notes[index];

    public IEnumerator<TNote> GetEnumerator() => _notes.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _notes.GetEnumerator();
}
