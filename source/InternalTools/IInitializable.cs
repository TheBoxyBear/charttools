namespace ChartTools.Collections
{
    public interface IInitializable
    {
        public bool Initialized { get; }
        public void Initialize();
    }
}
