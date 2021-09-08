namespace ChartTools
{
    /// <summary>
    /// Note played by drums
    /// </summary>
    public class DrumsNote : Note<DrumsFret>
    {
        private bool _isCymbal = false;
        /// <summary>
        /// <see langword="true"/> if the cymbal must be hit instead of the pad on supported drum sets
        /// </summary>
        /// <remarks><see cref="DrumsFret.Green5Lane"/> notes cannot be cymbal.</remarks>
        public bool IsCymbal
        {
            get => _isCymbal;
            set
            {
                if ((Fret == DrumsFret.Red || Fret == DrumsFret.Green5Lane) && value)
                    throw new InvalidOperationException("Red and 5-lane green notes cannot be cymbal.");

                _isCymbal = value;
            }
        }

        internal DrumsNote(DrumsFret note) : base(note) { }

        public bool IsKick => Fret is DrumsFret.Kick or DrumsFret.DoubleKick;
    }
}
