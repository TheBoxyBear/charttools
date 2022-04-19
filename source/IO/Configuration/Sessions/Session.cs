using ChartTools.Formatting;

using System;

namespace ChartTools.IO.Configuration.Sessions
{
    internal abstract class Session
    {
        public delegate bool DuplicateTrackObjectHandler(uint position, string objectType);

        public DuplicateTrackObjectHandler DuplicateTrackObjectProcedure => _duplicateTrackObjectProcedre is null ? _duplicateTrackObjectProcedre = Configuration.DuplicateTrackObjectPolicy switch
        {
            DuplicateTrackObjectPolicy.IncludeAll => DuplicateIncludeAll,
            DuplicateTrackObjectPolicy.IncludeFirst => DuplicateIncludeFirst,
            DuplicateTrackObjectPolicy.ThrowException => DuplicateException,
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.DuplicateTrackObjectPolicy)
        } : _duplicateTrackObjectProcedre;

        private DuplicateTrackObjectHandler? _duplicateTrackObjectProcedre;

        public virtual CommonConfiguration Configuration { get; } = new();
        public FormattingRules? Formatting { get; set; }

        public Session(FormattingRules? formatting) => Formatting = formatting;

        private static bool DuplicateIncludeAll(uint position, string objectType) => true;
        private static bool DuplicateIncludeFirst(uint position, string objectType) => false;
        private static bool DuplicateException(uint position, string objectType) => throw new Exception($"Duplicate {objectType} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.");
    }
}
