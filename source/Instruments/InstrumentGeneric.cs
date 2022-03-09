﻿using ChartTools.SystemExtensions.Linq;

using System;
using System.Linq;

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
            set => _easy = value is null ? null : value with { Difficulty = ChartTools.Difficulty.Easy, ParentInstrument = this };
        }
        private Track<TChord>? _easy;

        /// <summary>
        /// Medium track
        /// </summary>
        public new Track<TChord>? Medium
        {
            get => _medium;
            set => _medium = value is null ? null : value with { Difficulty = ChartTools.Difficulty.Medium, ParentInstrument = this };
        }
        private Track<TChord>? _medium;

        /// <summary>
        /// Hard track
        /// </summary>
        public new Track<TChord>? Hard
        {
            get => _hard;
            set => _hard = value is null ? null : value with { Difficulty = ChartTools.Difficulty.Hard, ParentInstrument = this };
        }
        private Track<TChord>? _hard;

        /// <summary>
        /// Expert track
        /// </summary>
        public new Track<TChord>? Expert
        {
            get => _expert;
            set => _expert = value is null ? null : value with { Difficulty = ChartTools.Difficulty.Expert, ParentInstrument = this };
        }
        private Track<TChord>? _expert;

        /// <summary>
        /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
        /// </summary>
        public override Track<TChord>? GetTrack(Difficulty difficulty) => difficulty switch
        {
            ChartTools.Difficulty.Easy => Easy,
            ChartTools.Difficulty.Medium => Medium,
            ChartTools.Difficulty.Hard => Hard,
            ChartTools.Difficulty.Expert => Expert,
            _ => throw CommonExceptions.GetUndefinedException(difficulty)
        };

        protected override Track<TChord>? GetEasy() => Easy;
        protected override Track<TChord>? GetMedium() => Medium;
        protected override Track<TChord>? GetHard() => Hard;
        protected override Track<TChord>? GetExpert() => Expert;

        public override Track<TChord>?[] GetTracks() => new Track<TChord>?[] { Easy, Medium, Hard, Expert };
        public override Track<TChord>[] GetNonEmptyTracks() => base.GetNonEmptyTracks().Cast<Track<TChord>>().ToArray();

        /// <summary>
        /// Sets a track for a given <see cref="Difficulty"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
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
