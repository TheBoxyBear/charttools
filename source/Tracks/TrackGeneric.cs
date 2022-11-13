namespace ChartTools;

/// <summary>
/// Set of chords for a instrument at a certain difficulty
/// </summary>
public record Track<TChord> : Track where TChord : IChord, new()
{
    /// <summary>
    /// Chords making up the difficulty track.
    /// </summary>
    public new List<TChord> Chords { get; } = new();
    /// <summary>
    /// Instrument the track is held in.
    /// </summary>
    public new Instrument<TChord>? ParentInstrument { get; init; }

    public override IChord CreateChord(uint position) => new TChord() { Position = position };

    /// <summary>
    /// Gets the chords as a read-only list of the base interface.
    /// </summary>
    protected override IReadOnlyList<IChord> GetChords() => (IReadOnlyList<IChord>)Chords;
    /// <summary>
    /// Gets the parent instrument as an instance of the base type.
    /// </summary>
    protected override Instrument? GetInstrument() => ParentInstrument;
}
