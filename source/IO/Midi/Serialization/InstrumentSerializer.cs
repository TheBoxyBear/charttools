using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Serialization;

internal class InstrumentSerializer<TChord> : TrackObjectGroupSerializer<Instrument<TChord>> where TChord : IChord, new()
{
    public InstrumentSerializer(string header, Instrument<TChord> content, WritingSession session) : base(header, content, session) { }

    protected override IEnumerable<IMidiEventMapping>[] LaunchMappers()
    {
        return new IEnumerable<IMidiEventMapping>[]
        {
            Content.GetMidiMapper(session).Map(Content).Cast<IMidiEventMapping>(),
            Content.ShareLocalEvents(session.Configuration.EventSource).OrderBy(e => e.Position).Cast<IMidiEventMapping>()
        };
    }
}
