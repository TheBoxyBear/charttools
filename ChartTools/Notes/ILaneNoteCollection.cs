namespace ChartTools;

public interface ILaneNoteCollection
{
	 /// <summary>
	 /// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
	 /// </summary>
	public bool OpenExclusivity { get; }

	public bool Contains(byte index);

	public bool Remove(byte index);

	public ILaneNote? this[byte index] { get; }

	public void Clear();

	public IEnumerable<ILaneNote> AsEnumerable();

	public IEnumerator<ILaneNote> GetEnumerator();
}
