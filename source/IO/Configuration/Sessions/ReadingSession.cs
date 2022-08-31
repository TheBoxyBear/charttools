using System;
using System.Collections.Generic;
using System.Linq;

using ChartTools.Formatting;
using ChartTools.Tools;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class ReadingSession : Session
    {
        public delegate bool TempolessAnchorHandler(Anchor anchor);

        public override ReadingConfiguration Configuration { get; }
        public TempolessAnchorHandler TempolessAnchorProcedure { get; private set; }
        public ReadingSession(ReadingConfiguration config, FormattingRules? formatting) : base(formatting)
        {
            Configuration = config;
            TempolessAnchorProcedure = anchor => (TempolessAnchorProcedure = Configuration.TempolessAnchorPolicy switch
            {
                TempolessAnchorPolicy.ThrowException => anchor => throw new Exception($"Tempo anchor at position {anchor.Position} does not have a parent tempo marker."),
                TempolessAnchorPolicy.Ignore => anchor => false,
                TempolessAnchorPolicy.Create => anchor => true,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.TempolessAnchorPolicy)
            })(anchor);
         }
    }
}
