namespace ChartTools;

/// <summary>
/// Marker that alters the tempo
/// </summary>
public class Tempo : ITrackObject
{
	/// <summary>
	/// Parent map the marker is contained
	/// </summary>
	public TempoMap? Map
	{
		get => m_map;
		internal set
		{
			if (value is not null)
				PositionSynced = false;

			m_map = value;
		}
	}
	private TempoMap? m_map;

	/// <inheritdoc cref="ITrackObject.Position" path="/summary"/>
	/// <remarks>Only refer to the position if <see cref="PositionSynced"/> is <see langword="true"/>.</remarks>
	public uint Position
	{
		get => m_position;
		set
		{
			m_position = value;

			if (Anchor is not null)
				PositionSynced = false;
		}
	}
	private uint m_position;

	/// <summary>
	/// New tempo in beats per minute
	/// </summary>
	public float Value { get; set; }

	/// <summary>
	/// Locks the tempo to a specific real-time position independent of the sync track.
	/// </summary>
	public TimeSpan? Anchor
	{
		get => m_anchor;
		set
		{
			bool valueNull = value is null;

			if (valueNull)
			{
				if (m_anchor is not null)
					Map?.RemoveAnchor(this);
			}
			else if (m_anchor is null)
					Map?.AddAnchor(this);

			m_anchor = value;
			PositionSynced = valueNull;
		}
	}
	private TimeSpan? m_anchor;

	/// <summary>
	/// Indicates if the tick position is up to date with <see cref="Anchor"/>.
	/// </summary>
	/// <remarks><see langword="true"/> if the marker has no anchor.</remarks>
	public bool PositionSynced { get; private set; } = true;

	/// <summary>
	/// Creates an instance of <see cref="Tempo"/>.
	/// </summary>
	public Tempo(uint position, float value)
	{
		Position = position;
		Value    = value;
	}
	public Tempo(TimeSpan anchor, float value) : this(0, value) => Anchor = anchor;

	internal void SyncPosition(uint position)
	{
		m_position     = position;
		PositionSynced = true;
	}
	internal void DesyncPosition() => PositionSynced = false;
}
