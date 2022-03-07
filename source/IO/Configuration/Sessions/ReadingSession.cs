namespace ChartTools.IO.Configuration.Sessions
{
    internal class ReadingSession : Session
    {
        public override ReadingConfiguration Configuration { get; }

        public ReadingSession(ReadingConfiguration config) => Configuration = config;
    }
}
