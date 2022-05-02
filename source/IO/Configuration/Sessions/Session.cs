using ChartTools.Formatting;

using System;

namespace ChartTools.IO.Configuration.Sessions
{
    internal abstract class Session
    {
        public delegate bool DuplicateTrackObjectHandler(uint position, string objectType, Func<bool> checkDuplciate);

        public DuplicateTrackObjectHandler DuplicateTrackObjectProcedure { get; private set; }
        public virtual CommonConfiguration Configuration { get; } = new();
        public FormattingRules? Formatting { get; set; }

        public Session(FormattingRules? formatting)
        {
            Formatting = formatting;
            DuplicateTrackObjectProcedure = (position, objectType, checkDuplicate) => (DuplicateTrackObjectProcedure = Configuration.DuplicateTrackObjectPolicy switch
            {
                DuplicateTrackObjectPolicy.IncludeAll => (_, _, _) => true,
                DuplicateTrackObjectPolicy.IncludeFirst => (_, _, checkDuplicate) => !checkDuplicate(),
                DuplicateTrackObjectPolicy.ThrowException => (position, objectType, checkDuplicate) => checkDuplicate()
                ? throw new Exception($"Duplicate {objectType} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.")
                : true,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.DuplicateTrackObjectPolicy)
            })(position, objectType, checkDuplicate);
        }
    }
}
