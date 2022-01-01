using ChartTools.IO.Chart.Providers;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools.Optimizing;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serializers
{
    internal class TrackSerializer<TChord, TProvider> : TrackObjectSerializer<Track<TChord>> where TChord : Chord where TProvider : ChordProvider<TChord>
    {
        public TrackSerializer(string header, Track<TChord> content) : base(header, content) { }

        protected override IEnumerable<TrackObjectProviderEntry>[] LaunchProviders()
        {
            ApplyOverlappingStarPowerPolicy(Content.StarPower, session!.Configuration.OverlappingStarPowerPolicy);

            // Convert solo and soloend events into star power
            if (session.Configuration.SoloNoStarPowerPolicy == SoloNoStarPowerPolicy.Convert && Content.StarPower.Count == 0 && Content.LocalEvents is not null)
            {
                StarPowerPhrase? starPower = null;

                foreach (LocalEvent e in Content.LocalEvents)
                    switch (e.EventType)
                    {
                        case LocalEventType.Solo:
                            if (starPower is not null)
                            {
                                starPower.Length = e.Position - starPower.Position;
                                Content.StarPower.Add(starPower);
                            }

                            starPower = new(e.Position);
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
                new StarPowerProvider().ProvideFor(Content.StarPower, session!),
                Content.LocalEvents is null ? Enumerable.Empty<TrackObjectProviderEntry>() : new EventProvider().ProvideFor(Content.LocalEvents!, session!)
            };
        }

        private static void ApplyOverlappingStarPowerPolicy(IEnumerable<StarPowerPhrase> starPower, OverlappingStarPowerPolicy policy)
        {
            switch (policy)
            {
                case OverlappingStarPowerPolicy.Cut:
                    starPower.CutLengths();
                    break;
                case OverlappingStarPowerPolicy.ThrowException:
                    foreach ((var previous, var current) in starPower.RelativeLoop())
                        if (Optimizer.LengthNeedsCut(previous!, current!))
                            throw new Exception($"Overlapping star power phrases at position {current!.Position}. Consider using {nameof(OverlappingStarPowerPolicy.Cut)} to avoid this error.");
                    break;
            }
        }
    }
}
