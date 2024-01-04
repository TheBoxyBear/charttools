using ChartTools.Events;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Configuration;

using DiffEnum = ChartTools.Difficulty;

namespace ChartTools;

/// <summary>
/// Base class for instruments
/// </summary>
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
                InstrumentIdentity.Vocals => InstrumentType.Vocals,
                _ => throw new InvalidDataException($"Instrument identity {InstrumentIdentity} does not belong to an instrument type.")
            };

            return _instrumentType.Value;
        }
    }
    private InstrumentType? _instrumentType;

    /// <summary>
    /// Set of special phrases applied to all difficulties
    /// </summary>
    public List<InstrumentSpecialPhrase> SpecialPhrases { get; set; } = [];

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
    /// Clears the track matching a difficulty.
    /// </summary>
    /// <param name="difficulty">Difficulty of the track</param>
    /// <returns>Newly created track</returns>
    public abstract Track ClearTrack(DiffEnum difficulty);

    /// <summary>
    /// Creates an array containing all tracks.
    /// </summary>
    public virtual Track?[] GetTracks() => new Track?[] { Easy, Medium, Hard, Expert };
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
            TrackObjectSource.Easy => collections[0],
            TrackObjectSource.Medium => collections[1],
            TrackObjectSource.Hard => collections[2],
            TrackObjectSource.Expert => collections[3],
            TrackObjectSource.Merge => collections.SelectMany(col => col).Distinct(),
            _ => throw new UndefinedEnumException(source)
        }).ToArray();

        foreach (var collection in collections)
        {
            collection.Clear();
            collection.AddRange(objects);
        }

        return objects;
    }

    public override string ToString() => InstrumentIdentity.ToString();
}
