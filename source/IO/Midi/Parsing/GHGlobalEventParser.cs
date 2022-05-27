using ChartTools.Events;
using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Midi.Parsing
{
    internal class GHGlobalEventParser : MidiParser
    {
        public override List<GlobalEvent> Result => GetResult(result);
        private readonly List<GlobalEvent> result = new();

        public GHGlobalEventParser(ReadingSession session) : base(session) { }

        protected override void HandleItem(MidiEvent item)
        {
            throw new NotImplementedException();
        }

        public override void ApplyToSong(Song song) => song.GlobalEvents = Result;
    }
}
