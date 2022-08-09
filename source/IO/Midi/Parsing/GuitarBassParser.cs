using ChartTools.IO.Configuration.Sessions;
using ChartTools.IO.Midi.Mapping;

using Melanchall.DryWetMidi.Core;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Midi.Parsing
{
    internal class GuitarBassParser : StandardInstrumentParser
    {
        private MidiInstrumentOrigin format;
        private readonly List<MidiMappingResult> mappings = new();
        private readonly Dictionary<int, uint?> openedBigRockPosition = new(from index in Enumerable.Range(1, 5) select new KeyValuePair<int, uint?>(index, null));

        public override MidiInstrumentOrigin Origin => format;
        protected override byte BigRockCount => 5;

        public GuitarBassParser(StandardInstrumentIdentity instrument, ReadingSession session) : base(instrument, new GuitarBassMapper(), session)
        {
            if (instrument is not StandardInstrumentIdentity.LeadGuitar or StandardInstrumentIdentity.Bass)
                throw new ArgumentException($"Instrument must be lead guitar or bass to use to use {nameof(GuitarBassParser)}.", nameof(instrument));
        }

        protected override bool CustomHandle(NoteEvent note)
        {
            var newFormat = MidiInstrumentOrigin.Unknown;
            var newMappings = mapper.MapNoteEvent(globalPosition, note, session);

            foreach (var mapping in mapper.MapNoteEvent(globalPosition, note, session))
            {
                if (mapping.Type is MappingType.Animation || note.NoteNumber == 116)
                    newFormat = MidiInstrumentOrigin.RockBand;
                else if (mapping.Type is MappingType.Special)
                {
                    if (mapping.Index is (byte)TrackSpecialPhraseType.Trill or (byte)TrackSpecialPhraseType.Tremolo)
                        newFormat = MidiInstrumentOrigin.RockBand;
                    else if (mapping.Difficulty is not Difficulty.Expert)
                        newFormat = MidiInstrumentOrigin.GuitarHero2;
                }
                else if (mapping.Type is MappingType.BigRock)
                    newFormat = MidiInstrumentOrigin.RockBand;

                if (format is MidiInstrumentOrigin.Unknown)
                    format = newFormat;
                else if (format != newFormat)
                    format = session.UncertainGuitarBassFormatProcedure(Instrument);

                mappings.AddRange(newMappings);
            }

            return true;
        }

        protected override void FinaliseParse()
        {
            if (format is MidiInstrumentOrigin.Unknown)
                format = session.UncertainGuitarBassFormatProcedure(Instrument);

            foreach (var mapping in mappings)
            {
                switch (mapping.Type)
                {
                    case MappingType.Note:
                        HandleNote(mapping);
                        break;
                    case MappingType.Modifier:
                        if (format.HasFlag(MidiInstrumentOrigin.RockBand) && mapping.Difficulty == Difficulty.Expert)
                            HandleModifier(mapping);
                        break;
                    case MappingType.Special when format.HasFlag(MidiInstrumentOrigin.RockBand):
                        if (mapping.Index is (byte)TrackSpecialPhraseType.Trill or (byte)TrackSpecialPhraseType.Tremolo || mapping.Difficulty is null)
                            HandleSpecial(mapping);
                        break;
                    case MappingType.Special when format.HasFlag(MidiInstrumentOrigin.GuitarHero2):
                        if (mapping.Index is not (byte)TrackSpecialPhraseType.Trill or (byte)TrackSpecialPhraseType.Tremolo)
                            HandleSpecial(mapping);
                        break;
                    case MappingType.BigRock:
                        HandleBigRock(mapping);
                        break;
                }
            }

            base.FinaliseParse();
        }
    }
}
