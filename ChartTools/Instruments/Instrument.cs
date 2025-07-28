using ChartTools.Events;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Configuration;

using System.Diagnostics;

using DiffEnum = ChartTools.Difficulty;

namespace ChartTools;

/// <summary>
/// Base class for instruments
/// </summary>
[DebuggerDisplay("{InstrumentIdentity}")]
public abstract record Instrument : IEmptyVerifiable
{
	/// <inheritdoc cref="IEmptyVerifiable.IsEmpty"/>
	public bool IsEmpty => GetExistingTracks().All(t => t.IsEmpty);

	/// <summary>
	/// Identity of the instrument the object belongs to
	/// </summary>
	public InstrumentIdentity InstrumentIdentity => GetIdentity();

	/// <summary>
	/// Type of instrument
	/// </summary>
	public InstrumentType InstrumentType
	{
		get
		{
			if (_instrumentType is not null)
				return _instrumentType.Value;

			_instrumentType = InstrumentIdentity switch
			{
				InstrumentIdentity.Drums => InstrumentType.Drums,
				InstrumentIdentity.LeadGuitar or InstrumentIdentity.RhythmGuitar or InstrumentIdentity.Bass or InstrumentIdentity.CoopGuitar or InstrumentIdentity.GHLBass or InstrumentIdentity.Keys => InstrumentType.Standard,
				InstrumentIdentity.GHLGuitar or InstrumentIdentity.GHLBass => InstrumentType.GHL,
				_ => throw new InvalidDataException($"Instrument identity {InstrumentIdentity} does not belong to an instrument type.")
			};

			return _instrumentType.Value;
		}
	}
	private InstrumentType? _instrumentType;

	/// <summary>
	/// Set of special phrases applied to all difficulties
	/// </summary>
	public List<InstrumentSpecialPhrase> SharedSpecialPhrases { get; set; } = [];

	/// <inheritdoc cref="InstrumentDifficultySet.GetDifficulty(InstrumentIdentity)"/>
	public sbyte? GetDifficulty(InstrumentDifficultySet difficulties) => difficulties.GetDifficulty(InstrumentIdentity);

	/// <inheritdoc cref="InstrumentDifficultySet.GetDifficulty(InstrumentIdentity)"/>
	public void SetDifficulty(InstrumentDifficultySet difficulties, sbyte? difficulty) => difficulties.SetDifficulty(InstrumentIdentity, difficulty);

	/// <summary>
	/// Easy track
	/// </summary>
	public Track? Easy => GetEasy();

	/// <summary>
	/// Medium track
	/// </summary>
	public Track? Medium => GetMedium();

	/// <summary>
	/// Hard track
	/// </summary>
	public Track? Hard => GetHard();

	/// <summary>
	/// Expert track
	/// </summary>
	public Track? Expert => GetExpert();

	/// <summary>
	/// Gets the track matching a difficulty.
	/// </summary>
	public abstract Track? GetTrack(DiffEnum difficulty);

	protected abstract Track? GetEasy();
	protected abstract Track? GetMedium();
	protected abstract Track? GetHard();
	protected abstract Track? GetExpert();

	/// <summary>
	/// Creates a track
	/// </summary>
	/// <param name="difficulty">Difficulty of the track</param>
	public abstract Track CreateTrack(DiffEnum difficulty);

	/// <summary>
	/// Removes a track.
	/// </summary>
	/// <param name="difficulty">Difficulty of the target track</param>
	public abstract bool RemoveTrack(DiffEnum difficulty);


	/// <summary>
	/// Creates an array containing all tracks.
	/// </summary>
	public virtual Track?[] GetTracks() => [Easy, Medium, Hard, Expert];

	/// <summary>
	/// Creates an array containing all tracks with data.
	/// </summary>
	public virtual IEnumerable<Track> GetExistingTracks() => GetTracks().NonNull().Where(t => !t.IsEmpty);

	protected abstract InstrumentIdentity GetIdentity();

	/// <summary>
	/// Gives all tracks the same local events.
	/// </summary>
	public LocalEvent[] ShareLocalEvents(TrackObjectSource source) => ShareEventsSpecial(source, track => track.LocalEvents);

	/// <summary>
	/// Gives all tracks the same special phrases
	/// </summary>
	public SpecialPhrase[] ShareSpecial(TrackObjectSource source) => ShareEventsSpecial(source, track => track.SpecialPhrases);

	private T[] ShareEventsSpecial<T>(TrackObjectSource source, Func<Track, List<T>> collectionGetter) where T : ITrackObject
	{
		var collections = GetExistingTracks().Select(track => collectionGetter(track)).ToArray();

		var objects = (source switch
		{
			TrackObjectSource.Easy   => collections[0],
			TrackObjectSource.Medium => collections[1],
			TrackObjectSource.Hard   => collections[2],
			TrackObjectSource.Expert => collections[3],
			TrackObjectSource.Merge  => collections.SelectMany(col => col).Distinct(),
			_                        => throw new UndefinedEnumException(source)
		}).ToArray();

		foreach (var collection in collections)
		{
			collection.Clear();
			collection.AddRange(objects);
		}

		return objects;
	}
}

/// <summary>
/// Set of tracks common to an instrument
/// </summary>
public abstract record Instrument<TChord> : Instrument where TChord : IChord
{
    /// <summary>
    /// Easy track
    /// </summary>
    public new Track<TChord>? Easy
    {
        get => m_easy;
        set => m_easy = value is null ? null : value with { Difficulty = Difficulty.Easy, ParentInstrument = this };
    }
    private Track<TChord>? m_easy;

    /// <summary>
    /// Medium track
    /// </summary>
    public new Track<TChord>? Medium
    {
        get => m_medium;
        set => m_medium = value is null ? null : value with { Difficulty = Difficulty.Medium, ParentInstrument = this };
    }
    private Track<TChord>? m_medium;

    /// <summary>
    /// Hard track
    /// </summary>
    public new Track<TChord>? Hard
    {
        get => m_hard;
        set => m_hard = value is null ? null : value with { Difficulty = Difficulty.Hard, ParentInstrument = this };
    }
    private Track<TChord>? m_hard;

    /// <summary>
    /// Expert track
    /// </summary>
    public new Track<TChord>? Expert
    {
        get => m_expert;
        set => m_expert = value is null ? null : value with { Difficulty = Difficulty.Expert, ParentInstrument = this };
    }
    private Track<TChord>? m_expert;

    /// <summary>
    /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
    /// </summary>
    public override Track<TChord>? GetTrack(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => Easy,
        Difficulty.Medium => Medium,
        Difficulty.Hard => Hard,
        Difficulty.Expert => Expert,
        _ => throw new UndefinedEnumException(difficulty)
    };

    /// <inheritdoc cref="Instrument.CreateTrack(Difficulty)"/>
    public override Track<TChord> CreateTrack(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => Easy = new(),
        Difficulty.Medium => Medium = new(),
        Difficulty.Hard => Hard = new(),
        Difficulty.Expert => Expert = new(),
        _ => throw new UndefinedEnumException(difficulty)
    };
    /// <inheritdoc cref="Instrument.RemoveTrack(Difficulty)"/>
    public override bool RemoveTrack(Difficulty difficulty)
    {
        bool found;

        switch (difficulty)
        {
            case Difficulty.Easy:
                found = m_easy is not null;
                m_easy = null;
                return found;
            case Difficulty.Medium:
                found = m_medium is not null;
                m_medium = null;
                return found;
            case Difficulty.Hard:
                found = m_hard is not null;
                m_hard = null;
                return found;
            case Difficulty.Expert:
                found = m_expert is not null;
                m_expert = null;
                return found;
            default:
                throw new UndefinedEnumException(difficulty);
        }
    }

    protected override Track<TChord>? GetEasy() => Easy;
    protected override Track<TChord>? GetMedium() => Medium;
    protected override Track<TChord>? GetHard() => Hard;
    protected override Track<TChord>? GetExpert() => Expert;

    public override Track<TChord>?[] GetTracks() => [Easy, Medium, Hard, Expert];

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
            Difficulty.Easy => m_easy = track with { ParentInstrument = this },
            Difficulty.Medium => m_medium = track with { ParentInstrument = this },
            Difficulty.Hard => m_hard = track with { ParentInstrument = this },
            Difficulty.Expert => m_expert = track with { ParentInstrument = this },
            _ => throw new UndefinedEnumException(track.Difficulty)
        };
}
