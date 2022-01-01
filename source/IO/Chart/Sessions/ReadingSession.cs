namespace ChartTools.IO.Chart.Sessions
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
