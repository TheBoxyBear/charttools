using System;

namespace ChartTools
{
    /// <summary>
    /// Exception thrown when a track that is not attached to an instrument is written to a file.
    /// </summary>
    public class DetachedTrackException : Exception
    {
        public Track Track { get; }

        public DetachedTrackException(Track track) : base($"Cannot replace track of difficulty {track.Difficulty} because it is not attached to an instrument.") => Track = track;
    }
}
