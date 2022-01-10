using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Configuration.Sessions
{
    internal class WritingSession : Session
    {
        public delegate IEnumerable<string> ChordLinesGetter(Chord? previous, Chord current);

        public override WritingConfiguration Configuration { get; }

        public ChordLinesGetter GetChordLines => _getChordLines is null ? _getChordLines = Configuration.UnsupportedModifierPolicy switch
        {
            UnsupportedModifierPolicy.IgnoreChord => (_, _) => Enumerable.Empty<string>(),
            UnsupportedModifierPolicy.ThrowException => (_, chord) => throw new Exception($"Chord at position {chord.Position} as an unsupported modifier for the chart format. Consider using a different {nameof(UnsupportedModifierPolicy)} to avoid this error."),
            UnsupportedModifierPolicy.IgnoreModifier => (_, chord) => chord.GetChartNoteData(),
            UnsupportedModifierPolicy.Convert => (previous, chord) => chord.GetChartModifierData(previous, this),
            _ => throw ConfigurationExceptions.UnsupportedPolicy(Configuration.UnsupportedModifierPolicy)
        } : _getChordLines;
        private ChordLinesGetter? _getChordLines;

        public uint HopoThreshold { get; set; }

        public WritingSession(WritingConfiguration config) : base(config) { }
    }

}
