using ChartTools.Animations;
using ChartTools.IO.Midi.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Midi.Parsing;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi;

internal class AnimParser(MidiReadingSession session) : MidiParser(session)
{
    public override HandPositionAnimationTrack Result => GetResult(result);
    private readonly HandPositionAnimationTrack result = new(HandPositionAnimationTrackIdentity.Guitar);

    public override void ApplyToSong(Song song) => song.Animations.Guitar = Result;

    protected override void HandleItem(MidiEvent item)
    {
        if (item is not NoteEvent note || (byte)note.NoteNumber is < 40 or > 59)
        {
            Session.HandleInvalidMidiEvent(globalPosition, item);
            return;
        }

        result.Add(new(0, AnimationMapper.GetHandPositionIndex((byte)note.NoteNumber)));
    }
}
