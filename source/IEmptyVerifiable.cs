namespace ChartTools
{
    /// <summary>
    /// Adds support for a property defining if an object is empty
    /// </summary>
    public interface IEmptyVerifiable
    {
        /// <summary>
        /// <see langword="true"/> if containing no data
        /// </summary>
        public bool IsEmpty { get; }
    }
}
