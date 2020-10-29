namespace ChartTools
{
    /// <summary>
    /// Base clas for chords
    /// </summary>
    public abstract class Chord : TrackObject
    {
        /// <summary>
        /// Creates an instance of <see cref="Chord"/>
        /// </summary>
        /// <param name="position"></param>
        public Chord(uint position) : base(position) { }

        /// <summary>
        /// Gets the data to write in a chart file.
        /// </summary>
        /// <returns>Enumerable of strings containing the data to add to each line</returns>
        internal abstract System.Collections.Generic.IEnumerable<string> GetChartData();
    }
}
