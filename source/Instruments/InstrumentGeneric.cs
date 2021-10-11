using ChartTools.IO;
using ChartTools.SystemExtensions.Linq;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of tracks common to an instrument
    /// </summary>
    public class Instrument<TChord> : Instrument where TChord : Chord
    {
        private Track<TChord> _easy = new(), _medium = new(), _hard = new(), _expert = new();
        /// <summary>
        /// Easy track
        /// </summary>
        public override Track<TChord> Easy => _easy;
        /// <summary>
        /// Medium track
        /// </summary>
        public override Track<TChord> Medium => _medium;
        /// <summary>
        /// Hard track
        /// </summary>
        public override Track<TChord> Hard => _hard;
        /// <summary>
        /// Expert track
        /// </summary>
        public override Track<TChord> Expert => _expert;

        /// <summary>
        /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
        /// </summary>
        public override Track<TChord> GetTrack(Difficulty difficulty) => difficulty switch
        {
            ChartTools.Difficulty.Easy => Easy,
            ChartTools.Difficulty.Medium => Medium,
            ChartTools.Difficulty.Hard => Hard,
            ChartTools.Difficulty.Expert => Expert,
            _ => throw CommonExceptions.GetUndefinedException(difficulty)
        };

        /// <summary>
        /// Sets a track for a given <see cref="Difficulty"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetTrack(Track<TChord> track, Difficulty difficulty)
        {
            if (track is null)
                throw new ArgumentNullException(nameof(track));

            switch (difficulty)
            {
                case ChartTools.Difficulty.Easy:
                    _easy = track;
                    break;
                case ChartTools.Difficulty.Medium:
                    _medium = track;
                    break;
                case ChartTools.Difficulty.Hard:
                    _hard = track;
                    break;
                case ChartTools.Difficulty.Expert:
                    _expert = track;
                    break;
            }
        }

        /// <summary>
        /// Gives all tracks the same local events.
        /// </summary>
        public void ShareLocalEvents(TrackObjectSource source)
        {
            if (source == TrackObjectSource.Seperate)
                return;

            LocalEvent?[]? events = ((IEnumerable<LocalEvent?>?)(source switch
            {
                TrackObjectSource.Easy => Easy.LocalEvents,
                TrackObjectSource.Medium => Medium.LocalEvents,
                TrackObjectSource.Hard => Hard.LocalEvents,
                TrackObjectSource.Expert => Expert.LocalEvents,
                TrackObjectSource.Merge => new Track<TChord>?[] { Easy, Medium, Hard, Expert }.NonNull().SelectMany(t => t.LocalEvents!).Distinct(),
                _ => throw CommonExceptions.GetUndefinedException(source)
            }))?.ToArray();

            if (events is null || events.Length == 0)
                return;

            Easy.LocalEvents = new(events!);
            Medium.LocalEvents = new(events!);
            Hard.LocalEvents = new(events!);
            Expert.LocalEvents = new(events!);
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
                TrackObjectSource.Easy => Easy.StarPower,
                TrackObjectSource.Medium => Medium.StarPower,
                TrackObjectSource.Hard => Hard.StarPower,
                TrackObjectSource.Expert => Expert.StarPower,
                TrackObjectSource.Merge => new Track<TChord>?[] { Easy, Medium, Hard, Expert }.NonNull().SelectMany(t => t.StarPower).Distinct(),
                _ => throw CommonExceptions.GetUndefinedException(source)
            })?.ToArray();

            if (starPower is null || starPower.Length == 0)
                return;

            Easy.StarPower = new(starPower.Length, starPower!);
            Medium.StarPower = new(starPower.Length, starPower!);
            Hard.StarPower = new(starPower.Length, starPower!);
            Expert.StarPower = new(starPower.Length, starPower!);
        }
    }
}
