using System;

using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

namespace ChartTools.IO.Midi.Parsing
{
    internal class GHGemsParser : StandardInstrumentParser
    {
        public override MidiInstrumentOrigin Origin => MidiInstrumentOrigin.GuitarHero1;

        public GHGemsParser(StandardInstrumentIdentity instrument, ReadingSession session) : base(instrument, new GHGemsMapper(), session)
        {
            if (instrument is not StandardInstrumentIdentity.LeadGuitar)
                throw new ArgumentException($"Instrument must be lead guitar to use {nameof(GHGemsParser)}.", nameof(instrument));
        }
    }
}
