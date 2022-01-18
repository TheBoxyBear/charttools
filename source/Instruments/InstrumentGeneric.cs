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
        public new Track<TChord> Easy
        {
            get => (Track<TChord>)base.Easy;
            set => base.Easy = value with { Difficulty = ChartTools.Difficulty.Easy, ParentInstrument = this };
        }
        /// <summary>
        /// Medium track
        /// </summary>
        public new Track<TChord> Medium
        {
            get => (Track<TChord>)base.Medium;
            set => base.Medium = value with { Difficulty = ChartTools.Difficulty.Medium, ParentInstrument = this };
        }
        /// <summary>
        /// Hard track
        /// </summary>
        public new Track<TChord> Hard
        {
            get => (Track<TChord>)base.Hard;
            set => base.Hard = value with { Difficulty = ChartTools.Difficulty.Hard, ParentInstrument = this };
        }
        /// <summary>
        /// Expert track
        /// </summary>
        public new Track<TChord> Expert
        {
            get => (Track<TChord>) base.Expert;
            set => base.Expert = value with { Difficulty = ChartTools.Difficulty.Expert, ParentInstrument = this };
        }

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
        public override Track<TChord>[] GetTracks() => base.GetTracks().Cast<Track<TChord>>().ToArray();
        public override Track<TChord>[] GetNonEmptyTracks() => base.GetNonEmptyTracks().Cast<Track<TChord>>().ToArray();

        /// <summary>
        /// Sets a track for a given <see cref="Difficulty"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetTrack(Track<TChord> track, Difficulty difficulty)
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
