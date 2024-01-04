using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi;

public class InvalidMidiEventTypeException : Exception
{
    public uint Position { get; }
    public string Type { get; }
    public override string Message => $"Invalid event of type {Type} at position {Position}.";

    public InvalidMidiEventTypeException(uint position, string type)
    {
        Position = position;
        Type = type;
    }
    public InvalidMidiEventTypeException(uint position, MidiEvent e)
    {
        Position = position;
        Type = e.GetType().Name;
    }
}
