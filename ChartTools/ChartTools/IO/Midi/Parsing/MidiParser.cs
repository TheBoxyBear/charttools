using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Parsing;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal abstract class MidiParser(MidiReadingSession session)
	: FileParser<MidiEvent>, ISongAppliable
{
	protected uint globalPosition;

	public MidiReadingSession Session { get; } = session;

	public abstract void ApplyToSong(Song song);
}
