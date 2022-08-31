using ChartTools.IO.Formatting;

using System;

namespace ChartTools.IO.Configuration.Sessions
{
    internal abstract class Session
    {
        public delegate bool DuplicateTrackObjectHandler(uint position, string objectType, Func<bool> checkDuplciate);
        public delegate bool SnappedNotesHandler(uint origin, uint position);

        public DuplicateTrackObjectHandler DuplicateTrackObjectProcedure { get; private set; }
        public SnappedNotesHandler SnappedNotesProcedure { get; private set; }
        public virtual CommonConfiguration Configuration { get; } = new();
        public FormattingRules? Formatting { get; set; }

        public Session(FormattingRules? formatting)
        {
            Formatting = formatting;

            DuplicateTrackObjectProcedure = (position, objectType, checkDuplicate) => (DuplicateTrackObjectProcedure = Configuration.DuplicateTrackObjectPolicy switch
            {
                DuplicateTrackObjectPolicy.ThrowException => (position, objectType, checkDuplicate) => checkDuplicate()
                ? throw new Exception($"Duplicate {objectType} on position {position}.") : true,
                DuplicateTrackObjectPolicy.IncludeAll => (_, _, _) => true,
                DuplicateTrackObjectPolicy.IncludeFirst => (_, _, checkDuplicate) => !checkDuplicate(),
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.DuplicateTrackObjectPolicy)
            })(position, objectType, checkDuplicate);
            SnappedNotesProcedure = (origin, position) => (SnappedNotesProcedure = Configuration.SnappedNotesPolicy switch
            {
                SnappedNotesPolicy.ThrowException => (origin, position) => throw new Exception($"Note at position {position} is within snapping distance from chord at position {origin}"),
                SnappedNotesPolicy.Snap => (_, _) => true,
                SnappedNotesPolicy.Ignore => (_, _) => false,
                _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.SnappedNotesPolicy)
            })(origin, position);
        }
    }
}
