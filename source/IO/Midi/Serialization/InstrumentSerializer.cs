using ChartTools.IO.Configuration.Sessions;
using ChartTools.Extensions.Linq;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Serialization
{
    internal class InstrumentSerializer : TrackObjectGroupSerializer<Instrument>
    {
        public InstrumentSerializer(string header, Instrument content, WritingSession session) : base(header, content, session) { }

        protected override IEnumerable<IMidiEventMapping>[] LaunchMappers()
        {
            return new IEnumerable<IMidiEventMapping>[]
            {
                (Content switch
                {
                    Instrument<StandardChord> standard => MapStandard(standard),
                }).OrderBy(m => m.Position),
                Content.ShareLocalEvents(session.Configuration.EventSource).OrderBy(e => e.Position).Cast<IMidiEventMapping>()
            };

            IEnumerable<IMidiEventMapping> MapStandard(Instrument<StandardChord> instrument)
            {
                InstrumentMapper<StandardChord> mapper;
                var format = Content.MidiOrigin;

                if (format is MidiInstrumentOrigin.GuitarHero1)
                    mapper = new GHGemsMapper();
                else
                {
                    if (format.HasFlag(MidiInstrumentOrigin.Unknown))
                        format = session.UncertainGuitarBassFormatProcedure((StandardInstrumentIdentity)Content.InstrumentIdentity, Content.MidiOrigin);

                    mapper = new GuitarBassMapper(format);
                }

                return mapper.Map(instrument).Cast<IMidiEventMapping>();
            }
        }
    }
}
