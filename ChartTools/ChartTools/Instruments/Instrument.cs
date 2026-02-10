using ChartTools.Events;
using ChartTools.Extensions.Enums;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Configuration;
using ChartTools.Meta;

using System.Diagnostics;

namespace ChartTools;

/// <summary>
/// Base class for instruments
/// </summary>
[DebuggerDisplay("{InstrumentIdentity}")]
public abstract record Instrument : IEmptyVerifiable
{
	/// <inheritdoc cref="IEmptyVerifiable.IsEmpty"/>
	public bool IsEmpty
		=> GetExistingTracks().All(static t => t.IsEmpty);

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
				InstrumentIdentity.StandardLeadGuitar or InstrumentIdentity.StandardRhythmGuitar or InstrumentIdentity.StandardBass or InstrumentIdentity.StandardCoopGuitar or InstrumentIdentity.GHLBass or InstrumentIdentity.StandardKeys => InstrumentType.Standard,
				InstrumentIdentity.GHLLeadGuitar or InstrumentIdentity.GHLBass => InstrumentType.GHL,
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
	public ref sbyte? GetDifficulty(InstrumentDifficultySet difficulties)
		=> ref difficulties.GetDifficulty(InstrumentIdentity);

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
	public abstract Track? GetTrack(SafeEnum<Difficulty> difficulty);

	protected abstract Track? GetEasy();

	protected abstract Track? GetMedium();

	protected abstract Track? GetHard();

	protected abstract Track? GetExpert();

	/// <summary>
	/// Creates a track
	/// </summary>
	/// <param name="difficulty">Difficulty of the track</param>
	public abstract Track CreateTrack(SafeEnum<Difficulty> difficulty);

	/// <summary>
	/// Removes a track.
	/// </summary>
	/// <param name="difficulty">Difficulty of the target track</param>
	public abstract bool RemoveTrack(SafeEnum<Difficulty> difficulty);

	/// <summary>
	/// Creates an array containing all tracks.
	/// </summary>
	public virtual Track?[] GetTracks()
		=> [Easy, Medium, Hard, Expert];

	/// <summary>
	/// Creates an array containing all tracks with data.
	/// </summary>
	public virtual IEnumerable<Track> GetExistingTracks()
		=> GetTracks().NonNull().Where(static t => !t.IsEmpty);

	protected abstract InstrumentIdentity GetIdentity();

	/// <summary>
	/// Gives all tracks the same local events.
	/// </summary>
	public LocalEvent[] ShareLocalEvents(TrackObjectSource source)
		=> ShareEventsSpecial(source, static track => track.LocalEvents);

	/// <summary>
	/// Gives all tracks the same special phrases.
	/// </summary>
	public SpecialPhrase[] ShareSpecial(TrackObjectSource source)
		=> ShareEventsSpecial(source, static track => track.SpecialPhrases);

	private T[] ShareEventsSpecial<T>(TrackObjectSource source, Func<Track, List<T>> collectionGetter)
		where T : ITrackObject
	{
		List<T>[] collections = [.. GetExistingTracks().Select(track => collectionGetter(track))];

		T[] objects = [.. source switch
		{
			TrackObjectSource.Easy   => collections[0],
			TrackObjectSource.Medium => collections[1],
			TrackObjectSource.Hard   => collections[2],
			TrackObjectSource.Expert => collections[3],
			TrackObjectSource.Merge  => collections.SelectMany(static col => col).Distinct(),
			_                        => throw new UndefinedEnumException(source)
		}];

		foreach (List<T> collection in collections)
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
public abstract record Instrument<TChord> : Instrument
	where TChord : Chord
{
	/// <summary>
	/// Easy track
	/// </summary>
	public new Track<TChord>? Easy
	{
		get;
		set => field = value is null ? null
			: value with { Difficulty = Difficulty.Easy, ParentInstrument = this };
	}

	/// <summary>
	/// Medium track
	/// </summary>
	public new Track<TChord>? Medium
	{
		get;
		set => field = value is null ? null
			: value with { Difficulty = Difficulty.Medium, ParentInstrument = this };
	}

	/// <summary>
	/// Hard track
	/// </summary>
	public new Track<TChord>? Hard
	{
		get;
		set => field = value is null ? null
			: value with { Difficulty = Difficulty.Hard, ParentInstrument = this };
	}

	/// <summary>
	/// Expert track
	/// </summary>
	public new Track<TChord>? Expert
	{
		get;
		set => field = value is null ? null
			: value with { Difficulty = Difficulty.Expert, ParentInstrument = this };
	}

	/// <summary>
	/// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
	/// </summary>
	public override Track<TChord>? GetTrack(SafeEnum<Difficulty> difficulty) => difficulty.Value switch
	{
		Difficulty.Easy   => Easy,
		Difficulty.Medium => Medium,
		Difficulty.Hard   => Hard,
		Difficulty.Expert => Expert,
		_ => throw new UndefinedEnumException(difficulty)
	};

	/// <inheritdoc cref="Instrument.CreateTrack(SafeEnum{Difficulty})"/>
	public override Track<TChord> CreateTrack(SafeEnum<Difficulty> difficulty)
		=> difficulty.Value switch
		{
			Difficulty.Easy   => Easy   = new(),
			Difficulty.Medium => Medium = new(),
			Difficulty.Hard   => Hard   = new(),
			Difficulty.Expert => Expert = new(),
			_ => throw new UndefinedEnumException(difficulty)
		};

	/// <inheritdoc cref="Instrument.RemoveTrack(SafeEnum{Difficulty})"/>
	public override bool RemoveTrack(SafeEnum<Difficulty> difficulty)
	{
		bool found;

		switch (difficulty.Value)
		{
			case Difficulty.Easy:
				found = Easy is not null;
				Easy  = null;
				return found;
			case Difficulty.Medium:
				found  = Medium is not null;
				Medium = null;
				return found;
			case Difficulty.Hard:
				found = Hard is not null;
				Hard  = null;
				return found;
			case Difficulty.Expert:
				found  = Expert is not null;
				Expert = null;
				return found;
			default:
				throw new UndefinedEnumException(difficulty);
		}
	}

	/// <summary>
	/// Exposes the <see cref="Easy"/> track to the base class.
	/// </summary>
	protected override Track<TChord>? GetEasy() => Easy;

	/// <summary>
	/// Exposes the <see cref="Medium"/> track to the base class.
	/// </summary>
	protected override Track<TChord>? GetMedium() => Medium;

	/// <summary>
	/// Exposes the <see cref="Hard"/> track to the base class.
	/// </summary>
	protected override Track<TChord>? GetHard() => Hard;

	/// <summary>
	/// Exposes the <see cref="Expert"/> track to the base class.
	/// </summary>
	protected override Track<TChord>? GetExpert() => Expert;

	public override Track<TChord>?[] GetTracks()
		=> [Easy, Medium, Hard, Expert];

	public override IEnumerable<Track<TChord>> GetExistingTracks()
		=> base.GetExistingTracks().Cast<Track<TChord>>();

	/// <summary>
	/// Sets a track for a given <see cref="Difficulty"/>.
	/// </summary>
	/// <returns>Track instance assigned to the instrument. Changed made to the passed reference will not be reflected in the instrument.</returns>
	/// <exception cref="ArgumentNullException"/>
	/// <exception cref="UndefinedEnumException"/>
	public Track<TChord> SetTrack(Track<TChord> track) => track is null
		? throw new ArgumentNullException(nameof(track))
		: track.Difficulty.Value switch
		{
			Difficulty.Easy   => Easy   = track with { ParentInstrument = this },
			Difficulty.Medium => Medium = track with { ParentInstrument = this },
			Difficulty.Hard   => Hard   = track with { ParentInstrument = this },
			Difficulty.Expert => Expert = track with { ParentInstrument = this },
			_  => throw new UndefinedEnumException(track.Difficulty)
		};
}
