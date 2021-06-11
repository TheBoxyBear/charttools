using ChartTools.Collections.Unique;
using ChartTools.IO;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools
{
    /// <summary>
    /// Set of tracks common to an instrument
    /// </summary>
    public class Instrument<TChord> : Instrument where TChord : Chord
    {
        /// <summary>
        /// Easy track
        /// </summary>
        public Track<TChord> Easy { get; set; } = null;
        /// <summary>
        /// Medium track
        /// </summary>
        public Track<TChord> Medium { get; set; } = null;
        /// <summary>
        /// Hard track
        /// </summary>
        public Track<TChord> Hard { get; set; } = null;
        /// <summary>
        /// Expert track
        /// </summary>
        public Track<TChord> Expert { get; set; } = null;

        /// <summary>
        /// Gets the <see cref="Track{TChord}"/> that matches a <see cref="Difficulty"/>
        /// </summary>
        public Track<TChord> GetTrack(Difficulty difficulty) => (Track<TChord>)GetType().GetProperty(difficulty.ToString()).GetValue(this);

        public void ShareLocalEvents(TrackObjectSource source)
        {
            if (source == TrackObjectSource.Seperate)
                return;

            LocalEvent[] events = ((IEnumerable<LocalEvent>)(source switch
            {
                TrackObjectSource.Easy => Easy?.LocalEvents,
                TrackObjectSource.Medium => Medium?.LocalEvents,
                TrackObjectSource.Hard => Hard?.LocalEvents,
                TrackObjectSource.Expert => Expert?.LocalEvents,
                TrackObjectSource.Merge => new UniqueEnumerable<LocalEvent>((e, other) => e.Equals(other), new Track<TChord>[] { Easy, Medium, Hard, Expert }.Select(t => t?.LocalEvents).ToArray()),
                _ => throw CommonExceptions.GetUndefinedException(source)
            })).ToArray();

            if (events.Length == 0)
                return;

#pragma warning disable S1121 // Assignments should not be made from within sub-expressions
            (Easy ??= new()).LocalEvents = new List<LocalEvent>(events);
            (Medium ??= new()).LocalEvents = new List<LocalEvent>(events);
            (Hard ??= new()).LocalEvents = new List<LocalEvent>(events);
            (Expert ??= new()).LocalEvents = new List<LocalEvent>(events);
#pragma warning restore S1121 // Assignments should not be made from within sub-expressions
        }
    }
}
