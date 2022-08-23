using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Serializaiton;

using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

using System;
using System.Collections.Generic;
using System.Text;

namespace ChartTools.IO.Midi.Serialization
{
    internal class InstrumentSerializer : TrackObjectGroupSerializer<Instrument>
    {
        public InstrumentSerializer(string header, Instrument content, WritingSession session) : base(header, content, session) { }

        protected override IEnumerable<IMidiEventMapping<MidiEvent>>[] LaunchMappers()
        {
            throw new NotImplementedException();
        }
    }
}
