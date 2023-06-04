using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools;

/// <summary>
/// Set of tracks common to an instrument
/// </summary>
public abstract record Instrument<TChord> : Instrument where TChord : IChord, new()
{
    /// <summary>
    /// Easy track
    /// </summary>
    public new Track<TChord> Easy
    {
        get => _easy;
        set => _easy = value with { Difficulty = Difficulty.Easy, ParentInstrument = this };
    }
    private Track<TChord> _easy;

    /// <summary>
    /// Medium track
    /// </summary>
    public new Track<TChord> Medium
    {
        get => _medium;
        set => _medium = value with { Difficulty = Difficulty.Medium, ParentInstrument = this };
    }
    private Track<TChord> _medium;

    /// <summary>
    /// Hard track
    /// </summary>
    public new Track<TChord> Hard
    {
        get => _hard;
        set => _hard = value with { Difficulty = Difficulty.Hard, ParentInstrument = this };
    }
    private Track<TChord> _hard;

    /// <summary>
    /// Expert track
    /// </summary>
    public new Track<TChord> Expert
    {
        get => _expert;
        set => _expert = value with { Difficulty = Difficulty.Expert, ParentInstrument = this };
    }
    private Track<TChord> _expert;

    public Instrument()
    {
        _easy = InitTrack(Difficulty.Easy);
        _medium = InitTrack(Difficulty.Medium);
        _hard = InitTrack(Difficulty.Hard);
        _expert = InitTrack(Difficulty.Expert);
    }

    private Track<TChord> InitTrack(Difficulty difficulty) => new() { Difficulty = difficulty, ParentInstrument = this };

    /// <summary>
    /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
    /// </summary>
    public override Track<TChord> GetTrack(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => Easy,
        Difficulty.Medium => Medium,
        Difficulty.Hard => Hard,
        Difficulty.Expert => Expert,
        _ => throw new UndefinedEnumException(difficulty)
    };

    protected override Track<TChord> GetEasy() => Easy;
    protected override Track<TChord> GetMedium() => Medium;
    protected override Track<TChord> GetHard() => Hard;
    protected override Track<TChord> GetExpert() => Expert;

    public override Track<TChord> ClearTrack(Difficulty difficulty)
    {
        ref Track<TChord> track = ref _easy;
        track = difficulty switch
        {
            Difficulty.Easy => _easy,
            Difficulty.Medium => _medium,
            Difficulty.Hard => _hard,
            Difficulty.Expert => _expert,
            _ => throw new UndefinedEnumException(difficulty),
        };

        return track = InitTrack(difficulty);
    }

    public override Track<TChord>[] GetTracks() => new Track<TChord>[] { Easy, Medium, Hard, Expert };
    public override IEnumerable<Track<TChord>> GetExistingTracks() => base.GetExistingTracks().Cast<Track<TChord>>();


    /// <summary>
    /// Sets a track for a given <see cref="Difficulty"/>.
    /// </summary>
    /// <returns>Track instance assigned to the instrument. Changed made to the passed reference will not be reflected in the instrument.</returns>
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="UndefinedEnumException"/>
    public Track<TChord> SetTrack(Track<TChord> track) => track is null
        ? throw new ArgumentNullException(nameof(track))
        : track.Difficulty switch
        {
            Difficulty.Easy => _easy = track with { ParentInstrument = this },
            Difficulty.Medium => _medium = track with { ParentInstrument = this },
            Difficulty.Hard => _hard = track with { ParentInstrument = this },
            Difficulty.Expert => _expert = track with { ParentInstrument = this },
            _ => throw new UndefinedEnumException(track.Difficulty)
        };

    internal abstract IInstrumentWriteMapper<TChord> GetMidiMapper(WritingSession session, AnimationSet animations);
}
