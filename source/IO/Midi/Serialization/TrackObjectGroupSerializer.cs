using ChartTools.Extensions.Linq;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Serializaiton;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Serialization;

internal abstract class TrackObjectGroupSerializer<T> : GroupSerializer<T, MidiEvent, IMidiEventMapping>
{
    public TrackObjectGroupSerializer(string header, T content, WritingSession session) : base(header, content, session) { }

    protected override IEnumerable<MidiEvent> CombineMapperResults(IEnumerable<IMidiEventMapping>[] mappings) => mappings.AlternateBy(mapping => mapping.Position).RelativeLoop().Select(pair =>
    {
        var previousPosition = pair.previous is null ? 0 : pair.previous.Position;
        return pair.current.ToMidiEvent(pair.current.Position - previousPosition);
    });
}
