namespace ChartTools.IO.Chart
{
    internal class ReadingSession
    {
        public ReadingConfiguration Configuration { get; }

        public ReadingSession(ReadingConfiguration? config)
        {
            Configuration = config ?? ChartParser.DefaultReadConfig;
        }
    }
}
