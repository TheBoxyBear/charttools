using ChartTools.Animations;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Serialization;

internal class InstrumentSerializer<TChord> : TrackObjectGroupSerializer<Instrument<TChord>> where TChord : IChord, new()
{
    public AnimationSet Animations { get; }

    public InstrumentSerializer(string header, Instrument<TChord> content, AnimationSet animations, MidiWritingSession session) : base(header, content, session) => Animations = animations;

    protected override IEnumerable<IMidiEventMapping>[] LaunchProviders()
    {
        return
        [
            Content.GetMidiMapper(Session, Animations).Map(Content).Cast<IMidiEventMapping>(),
            Content.ShareLocalEvents(Session.Configuration.EventSource).OrderBy(e => e.Position).Cast<IMidiEventMapping>()
        ];
    }
}
