namespace ChartTools;

/// <summary>
/// Note played by drums
/// </summary>
public class DrumsNote : LaneNote<DrumsLane>
{
    private bool _isCymbal = false;
    /// <summary>
    /// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
    /// </summary>
    /// <remarks><see cref="DrumsLane.Green5Lane"/> notes cannot be cymbal.</remarks>
    public bool IsCymbal
    {
        get => _isCymbal;
        set
        {
            if ((Lane == DrumsLane.Red || Lane == DrumsLane.Green5Lane) && value)
                throw new InvalidOperationException("Red and 5-lane green notes cannot be cymbal.");

            _isCymbal = value;
        }
    }

    public DrumsNote() : base() { }
    public DrumsNote(DrumsLane lane) : base(lane) { }

    /// <summary>
    /// Determines if the note is played by kicking
    /// </summary>
    public bool IsKick => Lane is DrumsLane.Kick or DrumsLane.DoubleKick;
}
