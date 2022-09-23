namespace ChartTools
{
    public interface INote : ILongObject
    {
        /// <summary>
        /// Numerical value of the note identity
        /// </summary>
        public byte Index { get; }
    }
}
