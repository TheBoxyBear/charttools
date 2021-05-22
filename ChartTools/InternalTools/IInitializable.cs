namespace ChartTools.Collections
{
    /// <summary>
    /// Defines an object that can be initialized
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Has already been initialized
        /// </summary>
        public bool Initialized { get; }
        /// <summary>
        /// Does required initialization if not already done.
        /// </summary>
        public void Initialize();
    }
}
