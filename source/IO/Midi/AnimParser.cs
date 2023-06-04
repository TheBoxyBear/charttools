using ChartTools.Animations;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;
using ChartTools.IO.Midi.Parsing;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi;

internal class AnimParser : MidiParser
{
    public override HandPositionAnimationTrack Result => GetResult(result);
    private readonly HandPositionAnimationTrack result = new(HandPositionAnimationTrackIdentity.Guitar);

    public AnimParser(ReadingSession session) : base(session) { }

    public override void ApplyToSong(Song song) => song.Animations.Guitar = Result;

    protected override void HandleItem(MidiEvent item)
    {
        if (item is not NoteEvent note || (byte)note.NoteNumber is < 40 or > 59)
        {
            session.InvalidMidiEventProcedure(globalPosition, item);
            return;
        }

        result.Add(new(0, AnimationMapper.GetHandPositionIndex((byte)note.NoteNumber)));
    }
}
