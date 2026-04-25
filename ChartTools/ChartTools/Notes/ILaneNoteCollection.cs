namespace ChartTools;

public interface ILaneNoteCollection
{
	/// <summary>
	/// If <see langword="true"/>, trying to combine an open note with other notes will remove the current ones.
	/// </summary>
	public bool OpenExclusivity { get; }

	public int Count { get; }

	public void Add(byte laneIndex, uint sustain = 0);

	public bool Contains(byte laneIndex);

	public bool Remove(byte laneIndex);

	public ILaneNote? this[byte laneIndex] { get; }

	public void Clear();

	public NoteProxy? Proxy(byte laneIndex);

	public IEnumerable<NoteProxy> ProxyAll();

	public IEnumerable<ILaneNote> AsEnumerable();

	public IEnumerator<ILaneNote> GetEnumerator();
}
