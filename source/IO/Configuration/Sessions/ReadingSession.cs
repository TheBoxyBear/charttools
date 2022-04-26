using ChartTools.Formatting;
using System;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class ReadingSession : Session
    {
        public delegate void UnopenedUnclosedObjectHandler(uint position, Action createOrInclude);

        public override ReadingConfiguration Configuration { get; }
        public UnopenedUnclosedObjectHandler HandleUnopened { get; private set; }
        public UnopenedUnclosedObjectHandler HandleUnclosed { get; private set; }

        public ReadingSession(ReadingConfiguration config, FormattingRules? formatting) : base(formatting)
        {
            Configuration = config;
            HandleUnopened = (position, create) => (HandleUnopened = Configuration.UnopenedTrackObjectPolicy switch
            {
                UnopenedTrackObjectPolicy.ThrowException => (position, _) => throw new Exception($"Object at position {position} closed before being opened."), // TODO Create exception
                UnopenedTrackObjectPolicy.Ignore => (_, _) => { }
                ,
                UnopenedTrackObjectPolicy.Create => (_, create) => create(),
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnopenedTrackObjectPolicy)
            })(position, create);
            HandleUnclosed = (position, include) => (HandleUnclosed = Configuration.UnclosedTracjObjectPolicy switch
            {
                UnclosedTrackObjectPolicy.ThrowException => throw new Exception($"Object at position {position} opened but never closed."), // TODO Create exception
                UnclosedTrackObjectPolicy.Ignore => (_, _) => { },
                UnclosedTrackObjectPolicy.Include => (_, include) => include()
            })(position, include);
         }
    }
}
