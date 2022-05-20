using System;
using System.Collections.Generic;
using System.Linq;

using ChartTools.Exceptions;

using DiffEnum = ChartTools.Difficulty;

namespace ChartTools
{
    /// <summary>
    /// Set of tracks common to an instrument
    /// </summary>
    public record Instrument<TChord> : Instrument where TChord : Chord
    {
        /// <summary>
        /// Easy track
        /// </summary>
        public new Track<TChord>? Easy
        {
            get => _easy;
            set => _easy = value is null ? null : value with { Difficulty = DiffEnum.Easy, ParentInstrument = this };
        }
        private Track<TChord>? _easy;

        /// <summary>
        /// Medium track
        /// </summary>
        public new Track<TChord>? Medium
        {
            get => _medium;
            set => _medium = value is null ? null : value with { Difficulty = DiffEnum.Medium, ParentInstrument = this };
        }
        private Track<TChord>? _medium;

        /// <summary>
        /// Hard track
        /// </summary>
        public new Track<TChord>? Hard
        {
            get => _hard;
            set => _hard = value is null ? null : value with { Difficulty = DiffEnum.Hard, ParentInstrument = this };
        }
        private Track<TChord>? _hard;

        /// <summary>
        /// Expert track
        /// </summary>
        public new Track<TChord>? Expert
        {
            get => _expert;
            set => _expert = value is null ? null : value with { Difficulty = DiffEnum.Expert, ParentInstrument = this };
        }
        private Track<TChord>? _expert;

        /// <summary>
        /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
        /// </summary>
        public override Track<TChord>? GetTrack(DiffEnum difficulty) => difficulty switch
        {
            DiffEnum.Easy => Easy,
            DiffEnum.Medium => Medium,
            DiffEnum.Hard => Hard,
            DiffEnum.Expert => Expert,
            _ => throw new UndefinedEnumException(difficulty)
        };

        /// <inheritdoc cref="Instrument.CreateTrack(DiffEnum)"/>
        public override Track CreateTrack(DiffEnum difficulty) => difficulty switch
        {
            DiffEnum.Easy => Easy = new(),
            DiffEnum.Medium => Medium = new(),
            DiffEnum.Hard => Hard = new(),
            DiffEnum.Expert => Expert = new(),
            _ => throw new UndefinedEnumException(difficulty)
        };
        /// <inheritdoc cref="Instrument.RemoveTrack(DiffEnum)"/>
        public override bool RemoveTrack(DiffEnum difficulty)
        {
            bool found;

            switch (difficulty)
            {
                case DiffEnum.Easy:
                    found = _easy is not null;
                    _easy = null;
                    return found;
                case DiffEnum.Medium:
                    found = _medium is not null;
                    _medium = null;
                    return found;
                case DiffEnum.Hard:
                    found = _hard is not null;
                    _hard = null;
                    return found;
                case DiffEnum.Expert:
                    found = _expert is not null;
                    _expert = null;
                    return found;
                default:
                    throw new UndefinedEnumException(difficulty);
            }
        }

        protected override Track<TChord>? GetEasy() => Easy;
        protected override Track<TChord>? GetMedium() => Medium;
        protected override Track<TChord>? GetHard() => Hard;
        protected override Track<TChord>? GetExpert() => Expert;

        public override Track<TChord>?[] GetTracks() => new Track<TChord>?[] { Easy, Medium, Hard, Expert };
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
                DiffEnum.Easy => _easy = track with { ParentInstrument = this },
                DiffEnum.Medium => _medium = track with { ParentInstrument = this },
                DiffEnum.Hard => _hard = track with { ParentInstrument = this },
                DiffEnum.Expert => _hard = track with { ParentInstrument = this },
                _ => throw new UndefinedEnumException(track.Difficulty)
            };
    }
}
