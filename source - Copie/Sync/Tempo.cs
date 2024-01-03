namespace ChartTools;

/// <summary>
/// Marker that alters the tempo
/// </summary>
public class Tempo : TrackObjectBase
{
    /// <summary>
    /// Parent map the marker is contained
    /// </summary>
    public TempoMap? Map
    {
        get => _map;
        internal set
        {
            if (value is not null)
                PositionSynced = false;

            _map = value;
        }
    }
    private TempoMap? _map;

    /// <inheritdoc cref="TrackObjectBase.Position" path="/summary"/>
    /// <remarks>Only refer to the position if <see cref="PositionSynced"/> is <see langword="true"/>.</remarks>
    public override uint Position
    {
        get => _position;
        set
        {
            _position = value;

            if (Anchor is not null)
                PositionSynced = false;
        }
    }
    private uint _position;

    /// <summary>
    /// New tempo in beats per minute
    /// </summary>
    public float Value { get; set; }

    /// <summary>
    /// Locks the tempo to a specific real-time position independent of the sync track.
    /// </summary>
    public TimeSpan? Anchor
    {
        get => _anchor;
        set
        {
            var valueNull = value is null;

            if (valueNull)
            {
                if (_anchor is not null)
                    Map?.RemoveAnchor(this);
            }
            else if (_anchor is null)
                    Map?.AddAnchor(this);

            _anchor = value;
            PositionSynced = valueNull;
        }
    }
    private TimeSpan? _anchor;

    /// <summary>
    /// Indicates if the tick position is up to date with <see cref="Anchor"/>.
    /// </summary>
    /// <remarks><see langword="true"/> if the marker has no anchor.</remarks>
    public bool PositionSynced { get; private set; } = true;

    /// <summary>
    /// Creates an instance of <see cref="Tempo"/>.
    /// </summary>
    public Tempo(uint position, float value) : base(position) => Value = value;
    public Tempo(TimeSpan anchor, float value) : this(0, value) => Anchor = anchor;

    internal void SyncPosition(uint position)
    {
        _position = position;
        PositionSynced = true;
    }
    internal void DesyncPosition() => PositionSynced = false;
}
