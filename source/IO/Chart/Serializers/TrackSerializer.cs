using ChartTools.Collections.Alternating;
using ChartTools.IO.Chart.Providers;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializers
{
    internal class TrackSerializer : TrackObjectGroupSerializer<Track>
    {
        public TrackSerializer(Track content, WritingSession session) : base(ChartFormatting.Header(content.ParentInstrument!.InstrumentIdentity, content.Difficulty), content, session) { }

        public override IEnumerable<string> Serialize() => new OrderedAlternatingEnumerable<uint, TrackObjectProviderEntry>(entry => entry.Position, LaunchProviders()).Select(entry => entry.Line);

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders()
        {
            ApplyOverlappingSpecialPhrasePolicy(Content.StarPower, session!.Configuration.OverlappingStarPowerPolicy);

            // Convert solo and soloend events into star power
            if (session.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert && Content.StarPower.Count == 0 && Content.LocalEvents is not null)
            {
                SpecicalPhrase? starPower = null;

                foreach (LocalEvent e in Content.LocalEvents)
                    switch (e.EventType)
                    {
                        case LocalEventType.Solo:
                            if (starPower is not null)
                            {
                                starPower.Length = e.Position - starPower.Position;
                                Content.StarPower.Add(starPower);
                            }

                            starPower = new(e.Position, SpecialPhraseType.StarPowerGain);
                            break;
                        case LocalEventType.SoloEnd when starPower is not null:

                            starPower.Length = e.Position - starPower.Position;
                            Content.StarPower.Add(starPower);

                            starPower = null;
                            break;
                    }

                Content.LocalEvents.RemoveWhere(e => e.EventType is LocalEventType.Solo or LocalEventType.SoloEnd);
            }

            return new IEnumerable<TrackObjectProviderEntry>[]
            {
                null!,
                new SpeicalPhraseProvider().ProvideFor(Content.StarPower, session!),
                Content.LocalEvents is null ? Enumerable.Empty<TrackObjectProviderEntry>() : new EventProvider().ProvideFor(Content.LocalEvents!, session!)
            };
        }

        private static void ApplyOverlappingSpecialPhrasePolicy(IEnumerable<SpecicalPhrase> specialPhrases, OverlappingSpecialPhrasePolicy policy)
        {
            switch (policy)
            {
                case OverlappingSpecialPhrasePolicy.Cut:
                    specialPhrases.CutLengths();
                    break;
                case OverlappingSpecialPhrasePolicy.ThrowException:
                    foreach ((var previous, var current) in specialPhrases.RelativeLoop())
                        if (Optimizer.LengthNeedsCut(previous!, current!))
                            throw new Exception($"Overlapping star power phrases at position {current!.Position}. Consider using {nameof(OverlappingSpecialPhrasePolicy.Cut)} to avoid this error.");
                    break;
            }
        }
    }
}
