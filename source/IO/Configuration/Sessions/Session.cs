using ChartTools.Formatting;

using System;
using System.Collections.Generic;

namespace ChartTools.IO.Configuration.Sessions
{
    internal abstract class Session
    {
        public delegate bool DuplicateTrackObjectHandler(uint position, ICollection<uint> ignored, string objectType);

        public DuplicateTrackObjectHandler DuplicateTrackObjectProcedure => _duplicateTrackObjectProcedre is null ? _duplicateTrackObjectProcedre = Configuration.DuplicateTrackObjectPolicy switch
        {
            DuplicateTrackObjectPolicy.IncludeAll => DuplicateIncludeAll,
            DuplicateTrackObjectPolicy.IncludeFirst => DuplicateIncludeFirst,
            DuplicateTrackObjectPolicy.ThrowException => DuplicateException,
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.DuplicateTrackObjectPolicy)
        } : _duplicateTrackObjectProcedre;

        private DuplicateTrackObjectHandler? _duplicateTrackObjectProcedre;

        public virtual CommonConfiguration Configuration { get; }
        public FormattingRules Formatting { get; } = new();

        public Session(CommonConfiguration config)
        {
            Configuration = config;
        }

        private static bool DuplicateIncludeAll(uint position, ICollection<uint> ignored, string objectType) => true;
        private static bool DuplicateIncludeFirst(uint position, ICollection<uint> ignored, string objectType)
        {
            if (ignored.Contains(position))
                return false;

            ignored.Add(position);
            return true;
        }
        private static bool DuplicateException(uint position, ICollection<uint> ignored, string objectType)
        {
            if (ignored.Contains(position))
                throw new Exception($"Duplicate {objectType} on position {position}. Consider using a different {nameof(DuplicateTrackObjectPolicy)} to avoid this error.");

            ignored.Add(position);
            return true;
        }
    }
}
