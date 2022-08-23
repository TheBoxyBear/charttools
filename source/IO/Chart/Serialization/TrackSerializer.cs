using ChartTools.Collections.Alternating;
using ChartTools.Events;
using ChartTools.IO.Chart.Entries;
using ChartTools.IO.Chart.Mapping;
using ChartTools.IO.Configuration;
using ChartTools.IO.Configuration.Sessions;
using ChartTools.SystemExtensions.Linq;
using ChartTools.Tools;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Chart.Serialization
{
    internal class TrackSerializer : TrackObjectGroupSerializer<Track>
    {
        public TrackSerializer(Track content, WritingSession session) : base(ChartFormatting.Header(content.ParentInstrument!.InstrumentIdentity, content.Difficulty), content, session) { }

        public override IEnumerable<string> Serialize() => new OrderedAlternatingEnumerable<uint, TrackObjectEntry>(entry => entry.Position, LaunchMappers()).Select(entry => entry.ToString());

        protected override IEnumerable<TrackObjectEntry>[] LaunchMappers()
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
                ChordMapper.Map(Content.Chords, session),
                Content.SpecialPhrases.Select(p => SpecialPhraseMapper.Map(p)),
                Content.LocalEvents is null ? Enumerable.Empty<TrackObjectEntry>() : Content.LocalEvents.Select(e => EventMapper.Map(e))
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
                    foreach ((var previous, var current) in specialPhrases.RelativeLoop())
                        if (Optimizer.LengthNeedsCut(previous!, current!))
                            throw new Exception($"Overlapping star power phrases at position {current!.Position}.");
                    break;
            }
        }
    }
}
