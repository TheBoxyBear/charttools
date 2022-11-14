using ChartTools.IO.Configuration.Sessions;

using Melanchall.DryWetMidi.Core;

namespace ChartTools.IO.Midi.Parsing;

internal record TitleSyncTraskResult(string Title, SyncTrack SyncTrack);

internal class TitleSyncTrackParser : MidiParser
{
    public override TitleSyncTraskResult Result => GetResult(result);
    protected readonly TitleSyncTraskResult result;
    private List<Tempo> tempos = new();

    private uint? previousSignaturePosition, previousTempoPosition;

    public TitleSyncTrackParser(string header, ReadingSession session) : base(session) => result = new(header, new());

    protected override void HandleItem(MidiEvent item)
    {
        globalPosition += (uint)item.DeltaTime;

        switch (item)
        {
            case TimeSignatureEvent ts:
                if (session.DuplicateTrackObjectProcedure(globalPosition, "time signature", () => previousSignaturePosition == globalPosition))
                {
                    result.SyncTrack.TimeSignatures.Add(new TimeSignature(globalPosition, ts.Numerator, ts.Denominator));
                    previousSignaturePosition = globalPosition;
                }
                break;
            case SetTempoEvent tempo:
                if (session.DuplicateTrackObjectProcedure(globalPosition, "tempo marker", () => previousTempoPosition == globalPosition))
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
