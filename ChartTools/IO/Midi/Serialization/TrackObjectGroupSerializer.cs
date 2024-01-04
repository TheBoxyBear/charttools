using ChartTools.Extensions.Linq;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Serialization;

internal abstract class TrackObjectGroupSerializer<T>(string header, T content, MidiWritingSession session)
    : GroupSerializer<T, MidiEvent, IMidiEventMapping>(header, content)
{
    public MidiWritingSession Session { get; } = session;

    protected override IEnumerable<MidiEvent> CombineMapperResults(IEnumerable<IMidiEventMapping>[] mappings) => mappings.AlternateBy(mapping => mapping.Position).RelativeLoop().Select(pair =>
    {
        var previousPosition = pair.previous is null ? 0 : pair.previous.Position;
        return pair.current.ToMidiEvent(pair.current.Position - previousPosition);
    });
}
