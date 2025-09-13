using System.Collections;

namespace ChartTools;

/// <summary>
/// Set of tempo markers that handles synchronism of anchored tempos
/// </summary>
public class TempoMap : IList<Tempo>
{
	private readonly List<Tempo> m_items = [];
	private readonly List<Tempo> m_anchors = [];

	public Tempo this[int index]
	{
		get => m_items[index];
		set => m_items[index] = value;
	}
	public int Count => m_items.Count;

	bool ICollection<Tempo>.IsReadOnly => false;

	/// <summary>
	/// Indicates if all anchored markers are synchronized.
	/// </summary>
	public bool Synchronized { get; private set; }

	private void AddBase(Tempo item)
	{
		item.Map = this;

		if (item.Anchor is not null)
			m_anchors.Add(item);
	}

	public void Add(Tempo item)
	{
		ArgumentNullException.ThrowIfNull(item);

		m_items.Add(item);

		AddBase(item);
		Desync();
	}

	public void AddRange(IEnumerable<Tempo> items)
	{
		foreach (Tempo item in items)
		{
			m_items.Add(item);
			AddBase(item);
		}

		Desync();
	}

	public void Clear() => m_items.Clear();

	public void Clear(bool detachMap)
	{
		if (detachMap)
			foreach (Tempo tempo in m_items)
				tempo.Map = null;

		m_items.Clear();
	}

	public bool Contains(Tempo item) => m_items.Contains(item);

	public void CopyTo(Tempo[] array, int arrayIndex) => m_items.CopyTo(array, arrayIndex);

	public int IndexOf(Tempo item) => m_items.IndexOf(item);

	public void Insert(int index, Tempo item)
	{
		m_items.Insert(index, item);

		AddBase(item);
		Desync();
	}

	public void InsertRange(int index, IEnumerable<Tempo> items)
	{
		foreach (Tempo item in items)
		{
			m_items.Insert(index, item);
			AddBase(item);
		}

		Desync();
	}

	public bool Remove(Tempo item) => Remove(item, false);

	public bool Remove(Tempo item, bool detachMap)
	{
		if (detachMap)
			item.Map = null;

		if (item.Anchor is not null)
			m_anchors.Remove(item);

		bool found = m_items.Remove(item);

		Desync();
		return found;
	}

	public void RemoveAt(int index)
	{
		m_items.RemoveAt(index);

		Tempo item = m_items[index];

		if (item.Anchor is not null)
			m_anchors.Remove(item);

		Desync();
	}
	public void RemoveAt(int index, bool detachMap)
	{
		if (detachMap)
		{
			Tempo tempo = m_items[index];
			tempo.Map = null;
		}

		m_items.RemoveAt(index);

		Tempo item = m_items[index];

		if (item.Anchor is not null)
			m_anchors.Remove(item);

		Desync();
	}

	public IEnumerator<Tempo> GetEnumerator() => m_items.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	/// <summary>
	/// Synchronizes anchored markers by calculating their tick position.
	/// </summary>
	/// <param name="resolution"></param>
	/// <param name="desyncedPreOrdered"></param>
	/// <exception cref="Exception"></exception>
	public void Synchronize(uint resolution, bool desyncedPreOrdered = false)
	{
		if (Synchronized)
			return;

		List<Tempo> synced = [];
		List<Tempo> desynced = [];

		// Split synced and desynced. Sync 0 anchors.
		foreach (Tempo tempo in m_items)
		{
			if (tempo.PositionSynced)
				synced.Add(tempo);
			else if (tempo.Anchor!.Value == TimeSpan.Zero)
			{
				tempo.SyncPosition(0);
				synced.Add(tempo);
			}
			else
				desynced.Add(tempo);
		}

		if (desynced.Count == 0)
			return;

		using IEnumerator<Tempo> syncedEnumerator = (desyncedPreOrdered ? (IEnumerable<Tempo>)synced : synced.OrderBy(t => t.Position)).GetEnumerator();

		if (!syncedEnumerator.MoveNext() || syncedEnumerator.Current.Position != 0)
			throw new Exception("A tempo marker at position or anchor zero is required to sync anchors.");

		using IEnumerator<Tempo> desyncedEnumerator = desynced.OrderBy(t => t.Anchor).GetEnumerator();

		syncedEnumerator.MoveNext();
		desyncedEnumerator.MoveNext();

		Tempo previous = syncedEnumerator.Current;
		ulong previousMs = 0ul;

		while (syncedEnumerator.MoveNext())
			while (TryInsertDesynced(syncedEnumerator.Current))
				if (!desyncedEnumerator.MoveNext())
					return;

		while (desyncedEnumerator.MoveNext())
			SyncAnchor();

		bool TryInsertDesynced(Tempo next)
		{
			float deltaMs = previous.Value * 50 / 3 * ((next.Position - previous.Position) / resolution);

			if (desyncedEnumerator.Current.Anchor!.Value.TotalMilliseconds - previousMs <= deltaMs)
			{
				SyncAnchor();
				return true;
			}

			previous = next;
			return false;
		}

		void SyncAnchor()
		{
			Tempo desynced = desyncedEnumerator.Current;
			desynced.SyncPosition((uint)((desynced.Anchor!.Value.TotalMilliseconds - previousMs) * previous.Value * resolution / 240000));

			previous = desynced;
		}
	}

	internal void Desync()
	{
		foreach (Tempo tempo in m_anchors)
			tempo.DesyncPosition();

		Synchronized = false;
	}

	internal void AddAnchor(Tempo item) => m_anchors.Add(item);

	internal void RemoveAnchor(Tempo item) => m_anchors.Remove(item);
}
