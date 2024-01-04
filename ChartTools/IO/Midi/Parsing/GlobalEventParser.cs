using ChartTools.Events;
using ChartTools.IO.Midi.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal class GlobalEventParser(MidiReadingSession session) : MidiParser(session)
{
    public override List<GlobalEvent> Result => GetResult(result);
    private readonly List<GlobalEvent> result = [];

    protected override void HandleItem(MidiEvent item)
    {
        globalPosition += (uint)item.DeltaTime;

        if (item is not TextEvent e)
        {
            session.HandleInvalidMidiEvent(globalPosition, item);
            return;
        }

        result.Add(new(globalPosition, e.Text));
    }

    public override void ApplyToSong(Song song) => song.GlobalEvents = Result;
}
