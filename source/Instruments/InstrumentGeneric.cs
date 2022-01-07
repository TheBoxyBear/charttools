using System;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of tracks common to an instrument
    /// </summary>
    public record Instrument<TChord> : Instrument where TChord : Chord
    {
        private Track<TChord> _easy = new Track<TChord>() with { Difficulty = ChartTools.Difficulty.Easy };
        private Track<TChord> _medium = new Track<TChord>() with { Difficulty = ChartTools.Difficulty.Medium };
        private Track<TChord> _hard = new Track<TChord>() with { Difficulty = ChartTools.Difficulty.Hard };
        private Track<TChord> _expert = new Track<TChord>() with { Difficulty = ChartTools.Difficulty.Expert };

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

            switch (difficulty)
            {
                case ChartTools.Difficulty.Easy:
                    _easy = track with
                    {
                        Difficulty = ChartTools.Difficulty.Easy,
                        ParentInstrument = this
                    };
                    break;
                case ChartTools.Difficulty.Medium:
                    _medium = track with
                    {
                        Difficulty = ChartTools.Difficulty.Medium,
                        ParentInstrument = this
                    };
                    break;
                case ChartTools.Difficulty.Hard:
                    _hard = track with
                    {
                        Difficulty = ChartTools.Difficulty.Hard,
                        ParentInstrument = this
                    };
                    break;
                case ChartTools.Difficulty.Expert:
                    _expert = track with
                    {
                        Difficulty = ChartTools.Difficulty.Expert,
                        ParentInstrument = this
                    };
                    break;
            }
        }
    }
}
