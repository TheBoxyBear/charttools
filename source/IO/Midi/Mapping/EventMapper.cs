using ChartTools.Events;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Mapping;

internal static class EventMapper
{
    public static T Map<T>(uint position, TextEvent e) where T : Event, new() => new() { Position = position, EventData = e.Text };
}
