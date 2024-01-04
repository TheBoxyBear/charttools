using ChartTools.IO.Midi.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal abstract class MidiParser(MidiReadingSession session) : FileParser<MidiEvent>, ISongAppliable
{
    protected uint globalPosition;

    public MidiReadingSession Session { get; } = session;

    public abstract void ApplyToSong(Song song);

    protected override Exception GetHandleException(MidiEvent item, Exception innerException)
    {
        return new NotImplementedException(innerException.Message, innerException);
    }

    protected override Exception GetFinalizeException(Exception innerException)
    {
        return new NotImplementedException(innerException.Message, innerException);
    }
}
