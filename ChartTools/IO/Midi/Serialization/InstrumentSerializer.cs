using ChartTools.Animations;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Serialization;

internal class InstrumentSerializer<TChord>(string header, Instrument<TChord> content, AnimationSet animations, MidiWritingSession session) : TrackObjectGroupSerializer<Instrument<TChord>>(header, content, session) where TChord : IChord, new()
{
    public AnimationSet Animations { get; } = animations;

    protected override IEnumerable<IMidiEventMapping>[] LaunchProviders()
    {
        return
        [
            Content.GetMidiMapper(Session, Animations).Map(Content).Cast<IMidiEventMapping>(),
            Content.ShareLocalEvents(Session.Configuration.EventSource).OrderBy(e => e.Position).Cast<IMidiEventMapping>()
        ];
    }
}
