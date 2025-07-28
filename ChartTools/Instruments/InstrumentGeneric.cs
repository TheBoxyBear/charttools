namespace ChartTools;

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
		Difficulty.Easy   => Easy,
		Difficulty.Medium => Medium,
		Difficulty.Hard   => Hard,
		Difficulty.Expert => Expert,
		_ => throw new UndefinedEnumException(difficulty)
	};

	/// <inheritdoc cref="Instrument.CreateTrack(Difficulty)"/>
	public override Track<TChord> CreateTrack(Difficulty difficulty) => difficulty switch
	{
		Difficulty.Easy   => Easy = new(),
		Difficulty.Medium => Medium = new(),
		Difficulty.Hard   => Hard = new(),
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

	public override Track<TChord>?[] GetTracks() => [ Easy, Medium, Hard, Expert ];

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
			Difficulty.Easy   => m_easy	  =	track with { ParentInstrument = this },
			Difficulty.Medium => m_medium = track with { ParentInstrument = this },
			Difficulty.Hard   => m_hard   =	track with { ParentInstrument = this },
			Difficulty.Expert => m_expert = track with { ParentInstrument = this },
			_                 => throw new UndefinedEnumException(track.Difficulty)
		};
}
