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

        public override void SetTrackNull(DiffEnum difficulty)
        {
            switch (difficulty)
            {
                case DiffEnum.Easy:
                    _easy = null;
                    break;
                case DiffEnum.Medium:
                    _medium = null;
                    break;
                case DiffEnum.Hard:
                    _hard = null;
                    break;
                case DiffEnum.Expert:
                    _expert = null;
                    break;
                default:
                    throw new UndefinedEnumException(difficulty);
            }
        }

        protected override Track<TChord>? GetEasy() => Easy;
        protected override Track<TChord>? GetMedium() => Medium;
        protected override Track<TChord>? GetHard() => Hard;
        protected override Track<TChord>? GetExpert() => Expert;

        public override Track<TChord>?[] GetTracks() => new Track<TChord>?[] { Easy, Medium, Hard, Expert };
        public override IEnumerable<Track<TChord>> GetNonEmptyTracks() => base.GetNonEmptyTracks().Cast<Track<TChord>>();

        /// <summary>
        /// Sets a track for a given <see cref="Difficulty"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        public void SetTrack(Track<TChord> track)
        {
            if (track is null)
                throw new ArgumentNullException(nameof(track));

            switch (track.Difficulty)
            {
                case ChartTools.Difficulty.Easy:
                    Easy = track;
                    break;
                case ChartTools.Difficulty.Medium:
                    Medium = track;
                    break;
                case ChartTools.Difficulty.Hard:
                    Hard = track;
                    break;
                case ChartTools.Difficulty.Expert:
                    Expert = track;
                    break;
            }
        }
    }
}
