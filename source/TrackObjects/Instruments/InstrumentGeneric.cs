using ChartTools.Collections.Unique;
using ChartTools.IO;
using ChartTools.SystemExtensions.Linq;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of tracks common to an instrument
    /// </summary>
    public class Instrument<TChord> : Instrument where TChord : Chord
    {
        /// <summary>
        /// Easy track
        /// </summary>
        public Track<TChord>? Easy { get; set; } = null;
        /// <summary>
        /// Medium track
        /// </summary>
        public Track<TChord>? Medium { get; set; } = null;
        /// <summary>
        /// Hard track
        /// </summary>
        public Track<TChord>? Hard { get; set; } = null;
        /// <summary>
        /// Expert track
        /// </summary>
        public Track<TChord>? Expert { get; set; } = null;

        protected override Track? GetEasy() => Easy;
        protected override Track? GetMedium() => Medium;
        protected override Track? GetHard() => Hard;
        protected override Track? GetExpert() => Expert;

        /// <summary>
        /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
        /// </summary>
        public new Track<TChord>? GetTrack(Difficulty difficulty) => GetType().GetProperty(difficulty.ToString())!.GetValue(this) as Track<TChord>;

        /// <summary>
        /// Gives all tracks the same local events.
        /// </summary>
        public void ShareLocalEvents(TrackObjectSource source)
        {
            if (source == TrackObjectSource.Seperate)
                return;

            LocalEvent?[]? events = ((IEnumerable<LocalEvent?>?)(source switch
            {
                TrackObjectSource.Easy => Easy?.LocalEvents,
                TrackObjectSource.Medium => Medium?.LocalEvents,
                TrackObjectSource.Hard => Hard?.LocalEvents,
                TrackObjectSource.Expert => Expert?.LocalEvents,
                TrackObjectSource.Merge => new Track<TChord>?[] { Easy, Medium, Hard, Expert }.NonNull().SelectMany(t => t.LocalEvents!).Distinct(),
                _ => throw CommonExceptions.GetUndefinedException(source)
            }))?.ToArray();

            if (events is null || events.Length == 0)
                return;

            (Easy ??= new()).LocalEvents = new List<LocalEvent>(events!);
            (Medium ??= new()).LocalEvents = new List<LocalEvent>(events!);
            (Hard ??= new()).LocalEvents = new List<LocalEvent>(events!);
            (Expert ??= new()).LocalEvents = new List<LocalEvent>(events!);
        }
        /// <summary>
        /// Gives all tracks the same star power
        /// </summary>
        public void ShareStarPower(TrackObjectSource source)
        {
            if (source == TrackObjectSource.Seperate)
                return;

            StarPowerPhrase?[]? starPower = (source switch
            {
                TrackObjectSource.Easy => Easy?.StarPower,
                TrackObjectSource.Medium => Medium?.StarPower,
                TrackObjectSource.Hard => Hard?.StarPower,
                TrackObjectSource.Expert => Expert?.StarPower,
                TrackObjectSource.Merge => new Track<TChord>?[] { Easy, Medium, Hard, Expert }.NonNull().SelectMany(t => t.StarPower).Distinct(),
                _ => throw CommonExceptions.GetUndefinedException(source)
            })?.ToArray();

            if (starPower is null || starPower.Length == 0)
                return;

            (Easy ??= new()).StarPower = new(starPower.Length, starPower!);
            (Medium ??= new()).StarPower = new(starPower.Length, starPower!);
            (Hard ??= new()).StarPower = new(starPower.Length, starPower!);
            (Expert ??= new()).StarPower = new(starPower.Length, starPower!);
        }
    }
}
