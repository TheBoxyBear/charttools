using DryWetFile = Melanchall.DryWetMidi.Core.MidiFile;

using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using System.Collections.Generic;
using System;

namespace ChartTools.IO.Midi.Serialization
{
    internal class TrackSerializer : TrackObjectGroupSerializer<Track>
    {
        private readonly DryWetFile file;

        public TrackSerializer(string header, Track content, DryWetFile file, WritingSession session) : base(header, content, session) => this.file = file;

        protected override IEnumerable<IMidiEventMapping>[] LaunchMappers()
        {
            throw new NotImplementedException();
        }
    }
}
