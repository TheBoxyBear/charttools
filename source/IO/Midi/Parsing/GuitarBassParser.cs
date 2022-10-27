using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Parsing
{
    internal class GuitarBassParser : StandardInstrumentParser
    {
        private readonly Dictionary<int, uint?> openedBigRockPosition = new(from index in Enumerable.Range(1, 5) select new KeyValuePair<int, uint?>(index, null));

        public override MidiInstrumentOrigin Origin => _origin;
        private MidiInstrumentOrigin _origin;

        protected override byte BigRockCount => 5;

        public GuitarBassParser(StandardInstrumentIdentity instrument, ReadingSession session) : base(instrument, new GuitarBassMapper(instrument), session)
        {
            if (instrument is not StandardInstrumentIdentity.LeadGuitar or StandardInstrumentIdentity.Bass)
                throw new ArgumentException($"Instrument must be lead guitar or bass to use to use {nameof(GuitarBassParser)}.", nameof(instrument));
        }

        protected override void FinaliseParse()
        {
            var origin = (mapper as GuitarBassMapper)!.Format;

            if (origin == MidiInstrumentOrigin.Unknown)
                _origin = session.UncertainGuitarBassFormatProcedure(Instrument, origin);

            base.FinaliseParse();
        }
    }
}
