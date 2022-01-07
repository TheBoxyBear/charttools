namespace ChartTools.IO.Chart.Serializers
{
    internal abstract class ChartSerializer<T> : ChartSerializer
    {
        public T Content { get; }

        public ChartSerializer(string header, T content) : base(header) => Content = content;
    }
}
