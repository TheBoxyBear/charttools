using ChartTools.Collections.Alternating;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Serializaiton;

using Melanchall.DryWetMidi.Core;

using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Serialization
{
    internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, MidiEvent, IMidiEventMapping<MidiEvent>>
    {
        public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

        protected override IEnumerable<MidiEvent> CombineMapperResults(IEnumerable<IMidiEventMapping<MidiEvent>>[] mappings) => new OrderedAlternatingEnumerable<uint, IMidiEventMapping<MidiEvent>>(mapper => mapper.Position, mappings).Select(mapping => mapping.ToMidiEvent(0));
    }
}
