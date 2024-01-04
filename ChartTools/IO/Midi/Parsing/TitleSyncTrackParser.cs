using ChartTools.IO.Midi.Configuration.Sessions;
using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal record TitleSyncTraskResult(string Title, SyncTrack SyncTrack);

internal class TitleSyncTrackParser(string header, MidiReadingSession session) : MidiParser(session)
{
    public override TitleSyncTraskResult Result => GetResult(result);
    protected readonly TitleSyncTraskResult result = new(header, new());
    private readonly List<Tempo> tempos = [];

    private uint? previousSignaturePosition, previousTempoPosition;

    protected override void HandleItem(MidiEvent item)
    {
        globalPosition += (uint)item.DeltaTime;

        switch (item)
        {
            case TimeSignatureEvent ts:
                if (session.HandleDuplicate(globalPosition, "time signature", () => previousSignaturePosition == globalPosition))
                {
                    result.SyncTrack.TimeSignatures.Add(new TimeSignature(globalPosition, ts.Numerator, ts.Denominator));
                    previousSignaturePosition = globalPosition;
                }
                break;
            case SetTempoEvent tempo:
                if (session.HandleDuplicate(globalPosition, "tempo marker", () => previousTempoPosition == globalPosition))
                {
                    tempos.Add(new Tempo(globalPosition, 60000000 / tempo.MicrosecondsPerQuarterNote));
                    previousTempoPosition = globalPosition;
                }
                break;
        }
    }

    public override void ApplyToSong(Song song)
    {
        var res = Result;

        song.Metadata = new() { Title = res.Title };
        song.SyncTrack = res.SyncTrack;
        song.SyncTrack.Tempo.AddRange(tempos);
    }
}
