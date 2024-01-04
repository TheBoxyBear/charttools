using ChartTools.Animations;

namespace ChartTools.IO.Midi.Mapping;

internal static class AnimationMapper
{
    public static byte GetHandPositionIndex(byte eventNumber) => eventNumber is > 39 and < 60 ? (byte)(eventNumber - 39) : throw new IndexOutOfRangeException($"Event number {eventNumber} is outside the range for hand positions.");
    public static byte MapHandPosition(HandPositionEvent handPosition) => (byte)(handPosition.Index + 39);
}
