using ChartTools.Events;
using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal class GlobalEventParser : MidiParser
{
    public override List<GlobalEvent> Result => GetResult(result);
    private readonly List<GlobalEvent> result = new();
    public GlobalEventParser(ReadingSession session) : base(session) { }

    protected override void HandleItem(MidiEvent item)
    {
        globalPosition += (uint)item.DeltaTime;

        if (item is not TextEvent e)
        {
            session.InvalidMidiEventProcedure(globalPosition, item);
            return;
        }

        result.Add(new(globalPosition, e.Text));
    }

    public override void ApplyToSong(Song song) => song.GlobalEvents = Result;
}
