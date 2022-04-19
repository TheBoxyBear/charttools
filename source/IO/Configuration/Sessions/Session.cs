using ChartTools.Formatting;

using System;

namespace ChartTools.IO.Configuration.Sessions
{
    internal abstract class Session
    {
        public delegate bool DuplicateTrackObjectHandler(uint position, string objectType, Func<bool> checkDuplciate);

        public DuplicateTrackObjectHandler DuplicateTrackObjectProcedure => _duplicateTrackObjectProcedre is null ? _duplicateTrackObjectProcedre = Configuration.DuplicateTrackObjectPolicy switch
        {
            DuplicateTrackObjectPolicy.IncludeAll => DuplicateIncludeAll,
            DuplicateTrackObjectPolicy.IncludeFirst => DuplicateIncludeFirst,
            DuplicateTrackObjectPolicy.ThrowException => DuplicateThrowException,
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.DuplicateTrackObjectPolicy)
        } : _duplicateTrackObjectProcedre;

        private DuplicateTrackObjectHandler? _duplicateTrackObjectProcedre;

        private static DuplicateTrackObjectHandler DuplicateIncludeAll = (_, _, _) => true;
        private static DuplicateTrackObjectHandler DuplicateIncludeFirst = (_, _, checkDuplicate) => !checkDuplicate();
        private static DuplicateTrackObjectHandler DuplicateThrowException = (position, objectType, checkDuplicate) => checkDuplicate()
        ? throw new Exception($"Duplicate {objectType} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.")
        : true;

        public virtual CommonConfiguration Configuration { get; } = new();
        public FormattingRules? Formatting { get; set; }

        public Session(FormattingRules? formatting) => Formatting = formatting;
    }
}
