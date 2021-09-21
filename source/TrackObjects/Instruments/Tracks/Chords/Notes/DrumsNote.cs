using System;

namespace ChartTools
{
    /// <summary>
    /// Note played by drums
    /// </summary>
    public class DrumsNote : Note<DrumsLane>
    {
        private bool _isCymbal = false;
        /// <summary>
        /// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
        /// </summary>
        /// <remarks><see cref="DrumsLane.Green5Lane"/> notes cannot be cymbal.</remarks>
        public bool IsCymbal
        {
            get => _isCymbal;
            set
            {
                if ((Lane == DrumsLane.Red || Lane == DrumsLane.Green5Lane) && value)
                    throw new InvalidOperationException("Red and 5-lane green notes cannot be cymbal.");

                _isCymbal = value;
            }
        }

        internal DrumsNote(DrumsLane note) : base(note) { }

        public bool IsKick => Lane is DrumsLane.Kick or DrumsLane.DoubleKick;
    }
}
