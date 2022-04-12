using ChartTools.Formatting;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class ReadingSession : Session
    {
        public override ReadingConfiguration Configuration { get; }

        public ReadingSession(ReadingConfiguration config, FormattingRules? formatting) : base(formatting) => Configuration = config;
    }
}
