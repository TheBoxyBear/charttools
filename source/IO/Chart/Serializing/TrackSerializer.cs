using ChartTools.Events;
using ChartTools.Extensions.Collections;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.Tools;

namespace ChartTools.IO.Chart.Serializing;

internal class TrackSerializer : TrackObjectGroupSerializer<Track>
{
    public TrackSerializer(Track content, WritingSession session) : base(ChartFormatting.Header(content.ParentInstrument!.InstrumentIdentity, content.Difficulty), content, session) { }

    public override IEnumerable<string> Serialize() => new OrderedAlternatingEnumerable<TrackObjectEntry, uint>(entry => entry.Position, LaunchProviders()).Select(entry => entry.ToString());

    protected override IEnumerable<TrackObjectEntry>[] LaunchProviders()
    {
        ApplyOverlappingSpecialPhrasePolicy(Content.SpecialPhrases, session.Configuration.OverlappingStarPowerPolicy);

        // Convert solo and soloend events into star power
        if (session.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert && Content.SpecialPhrases.Count == 0 && Content.LocalEvents is not null)
        {
            TrackSpecialPhrase? starPower = null;

            foreach (var e in Content.LocalEvents)
                switch (e.EventType)
                {
                    case EventTypeHelper.Local.Solo:
                        if (starPower is not null)
                        {
                            starPower.Length = e.Position - starPower.Position;
                            Content.SpecialPhrases.Add(starPower);
                        }

                        starPower = new(e.Position, TrackSpecialPhraseType.StarPowerGain);
                        break;
                    case EventTypeHelper.Local.SoloEnd when starPower is not null:

                        starPower.Length = e.Position - starPower.Position;
                        Content.SpecialPhrases.Add(starPower);

                        starPower = null;
                        break;
                }

            Content.LocalEvents.RemoveWhere(e => e.IsSoloEvent);
        }

        return new IEnumerable<TrackObjectEntry>[]
        {
            new ChordProvider().ProvideFor(Content.Chords.Cast<LaneChord>(), session),
            new SpeicalPhraseProvider().ProvideFor(Content.SpecialPhrases, session!),
            Content.LocalEvents is null ? Enumerable.Empty<TrackObjectEntry>() : new EventProvider().ProvideFor(Content.LocalEvents!, session!)
        };
    }

    private static void ApplyOverlappingSpecialPhrasePolicy(IEnumerable<TrackSpecialPhrase> specialPhrases, OverlappingSpecialPhrasePolicy policy)
    {
        switch (policy)
        {
            case OverlappingSpecialPhrasePolicy.Cut:
                specialPhrases.CutLengths();
                break;
            case OverlappingSpecialPhrasePolicy.ThrowException:
                foreach ((var previous, var current) in specialPhrases.RelativeLoopSkipFirst())
                    if (Optimizer.LengthNeedsCut(previous, current))
                        throw new Exception($"Overlapping star power phrases at position {current!.Position}.");
                break;
        }
    }
}
